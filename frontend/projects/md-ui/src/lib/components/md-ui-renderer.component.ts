import {
  Component,
  Input,
  ViewChild,
  ViewContainerRef,
  ComponentRef,
  Type,
  ChangeDetectorRef,
  OnChanges,
  SimpleChanges,
  OnDestroy,
  OnInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewTreeNode } from '../models';
import { MdUiService } from '../services';
import { MdTextComponent } from './md-text.component';
import { MdButtonComponent } from './md-button.component';
import { MdColumnComponent } from './layout/md-column.component';
import { MdRowComponent } from './layout/md-row.component';
import { MdContainerComponent } from './layout/md-container.component';
import { MdTextFieldComponent } from './input/md-textfield.component';
import { MdSwitchComponent } from './input/md-switch.component';
import { MdCheckboxComponent } from './input/md-checkbox.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'md-ui-renderer',
  template: `
    <div class="md-ui-root">
      <ng-container #container></ng-container>
    </div>
  `,
  styles: [`
    .md-ui-root {
      width: 100%;
      height: 100%;
    }
  `],
  standalone: true,
  imports: [CommonModule]
})
export class MdUiRendererComponent implements OnInit, OnChanges, OnDestroy {
  @Input() viewTree: ViewTreeNode | null = null;
  @Input() sessionId: string | null = null;
  @ViewChild('container', { read: ViewContainerRef, static: true }) container!: ViewContainerRef;

  private componentRefMap = new Map<string, { ref: ComponentRef<any>, container: ViewContainerRef }>();
  private previousViewTree: ViewTreeNode | null = null;

  private componentMap: { [key: string]: Type<any> } = {
    'MdText': MdTextComponent,
    'MdButton': MdButtonComponent,
    'MdColumn': MdColumnComponent,
    'MdRow': MdRowComponent,
    'MdContainer': MdContainerComponent,
    'MdTextField': MdTextFieldComponent,
    'MdSwitch': MdSwitchComponent,
    'MdCheckbox': MdCheckboxComponent,
  };

  constructor(
    private mdUiService: MdUiService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Render initial tree if provided
    if (this.viewTree) {
      this.render();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Handle view tree changes
    if (changes['viewTree']) {
      if (this.viewTree) {
        this.render();
      } else {
        this.clearComponents();
      }
    }
  }

  ngOnDestroy(): void {
    this.clearComponents();
  }

  private clearComponents(): void {
    this.componentRefMap.forEach(({ ref }) => ref.destroy());
    this.componentRefMap.clear();
    this.container.clear();
    this.previousViewTree = null;
  }

  private render(): void {
    if (!this.viewTree) return;

    if (!this.previousViewTree || this.componentRefMap.size === 0) {
      // First render - create everything
      this.container.clear();
      this.componentRefMap.clear();
      this.createNode(this.viewTree, this.container, null);
      this.previousViewTree = this.deepClone(this.viewTree);
    } else {
      // Incremental update - diff and patch
      this.diffAndPatch(this.viewTree, this.previousViewTree, this.container, null);
      this.previousViewTree = this.deepClone(this.viewTree);
      this.cdr.detectChanges();
    }
  }

  private diffAndPatch(
    newNode: ViewTreeNode,
    oldNode: ViewTreeNode | null,
    parentContainer: ViewContainerRef,
    parentNodeId: string | null
  ): void {
    // No old node - create new
    if (!oldNode) {
      this.createNode(newNode, parentContainer, parentNodeId);
      return;
    }

    // Different node IDs or types - recreate
    if (newNode.nodeId !== oldNode.nodeId || newNode.componentType !== oldNode.componentType) {
      this.destroyNodeRecursive(oldNode);
      this.createNode(newNode, parentContainer, parentNodeId);
      return;
    }

    // Same node - update
    const componentData = this.componentRefMap.get(newNode.nodeId);
    if (!componentData) {
      // Component not found - recreate
      this.createNode(newNode, parentContainer, parentNodeId);
      return;
    }

    // Update properties
    this.updateProperties(componentData.ref, newNode.properties, oldNode.properties);

    // Update children
    this.updateChildren(componentData.ref, newNode, oldNode);
  }

  private createNode(
    node: ViewTreeNode,
    parentContainer: ViewContainerRef,
    parentNodeId: string | null
  ): ComponentRef<any> | null {
    const componentType = this.componentMap[node.componentType];

    if (!componentType) {
      console.error(`[MdUiRenderer] Unknown component type: ${node.componentType}`);
      return null;
    }

    // Create component
    const componentRef = parentContainer.createComponent(componentType);

    // Store reference with container info
    this.componentRefMap.set(node.nodeId, {
      ref: componentRef,
      container: parentContainer
    });

    // Set properties
    this.updateProperties(componentRef, node.properties, {});
    componentRef.instance.nodeId = node.nodeId;

    // Bind events (only once during creation)
    this.bindEvents(componentRef, node);

    // Create children
    if (node.children && node.children.length > 0) {
      const childContainer = this.getChildContainer(componentRef);

      node.children.forEach(childNode => {
        this.createNode(childNode, childContainer, node.nodeId);
      });
    }

    componentRef.changeDetectorRef.detectChanges();
    return componentRef;
  }

  private updateProperties(componentRef: ComponentRef<any>, newProperties: any, oldProperties: any): boolean {
    // Save focus state before updating
    const activeElement = document.activeElement;
    const hasFocus = activeElement && this.isDescendant(componentRef.location.nativeElement, activeElement);

    let hasChanges = false;

    const allKeys = new Set([...Object.keys(newProperties), ...Object.keys(oldProperties || {})]);

    allKeys.forEach(key => {
      const newValue = newProperties[key];
      const oldValue = oldProperties?.[key];

      const isEqual = this.deepEqual(newValue, oldValue);

      if (!isEqual) {
        // Update the property
        componentRef.instance[key] = newValue;
        hasChanges = true;
      }
    });

    if (hasChanges) {
      // Mark this component and all ancestors for check
      componentRef.changeDetectorRef.markForCheck();
      // Immediately detect changes on this component
      componentRef.changeDetectorRef.detectChanges();
    }

    // Restore focus if needed
    if (hasFocus && activeElement instanceof HTMLElement) {
      // Use setTimeout to ensure DOM has been updated
      setTimeout(() => {
        if (document.activeElement !== activeElement) {
          activeElement.focus();
        }
      }, 0);
    }

    return hasChanges;
  }

  private isDescendant(parent: any, child: any): boolean {
    let node = child;
    while (node !== null) {
      if (node === parent) {
        return true;
      }
      node = node.parentNode;
    }
    return false;
  }

  private updateChildren(componentRef: ComponentRef<any>, newNode: ViewTreeNode, oldNode: ViewTreeNode): void {
    const newChildren = newNode.children || [];
    const oldChildren = oldNode.children || [];

    // Build maps for quick lookup
    const oldChildMap = new Map<string, ViewTreeNode>();
    oldChildren.forEach(child => oldChildMap.set(child.nodeId, child));

    const newChildIds = new Set(newChildren.map(c => c.nodeId));

    const childContainer = this.getChildContainer(componentRef);

    // Remove children that no longer exist
    oldChildren.forEach(oldChild => {
      if (!newChildIds.has(oldChild.nodeId)) {
        this.destroyNodeRecursive(oldChild);
      }
    });

    // Process each new child
    newChildren.forEach((newChild) => {
      const oldChild = oldChildMap.get(newChild.nodeId);
      this.diffAndPatch(newChild, oldChild || null, childContainer, newNode.nodeId);
    });
  }

  private destroyNodeRecursive(node: ViewTreeNode): void {
    // First destroy all children
    if (node.children) {
      node.children.forEach(child => this.destroyNodeRecursive(child));
    }

    // Then destroy the node itself
    const componentData = this.componentRefMap.get(node.nodeId);
    if (componentData) {
      componentData.ref.destroy();
      this.componentRefMap.delete(node.nodeId);
    }
  }

  private bindEvents(componentRef: ComponentRef<any>, node: ViewTreeNode): void {
    if (!node.events || node.events.length === 0) return;

    node.events.forEach(eventName => {
      if (eventName === 'click') {
        componentRef.instance.click?.subscribe(() => {
          if (this.sessionId) {
            this.mdUiService.sendEvent(this.sessionId, node.nodeId, 'click');
          }
        });
      } else if (eventName === 'changed') {
        componentRef.instance.valueChange?.subscribe((value: any) => {
          if (this.sessionId) {
            this.mdUiService.sendEvent(this.sessionId, node.nodeId, 'changed', { value });
          }
        });
      }
    });
  }

  private getChildContainer(componentRef: ComponentRef<any>): ViewContainerRef {
    return componentRef.injector.get(ViewContainerRef);
  }

  private deepEqual(a: any, b: any): boolean {
    // Same reference or same primitive value
    if (a === b) return true;

    // null/undefined check
    if (a == null || b == null) return false;

    // Different types
    if (typeof a !== typeof b) return false;

    // Handle arrays
    if (Array.isArray(a) && Array.isArray(b)) {
      if (a.length !== b.length) return false;
      return a.every((item, index) => this.deepEqual(item, b[index]));
    }

    // Handle objects
    if (typeof a === 'object' && typeof b === 'object') {
      const keysA = Object.keys(a);
      const keysB = Object.keys(b);

      if (keysA.length !== keysB.length) return false;

      return keysA.every(key => this.deepEqual(a[key], b[key]));
    }

    // Primitives (string, number, boolean, etc.) - already checked with ===
    return a === b;
  }

  private deepClone(obj: any): any {
    return JSON.parse(JSON.stringify(obj));
  }
  
  /**
   * Apply property updates received from the backend
   */
  public applyPropertyUpdates(batch: any): void {
    if (!batch.updates || batch.updates.length === 0) {
      return;
    }
    
    batch.updates.forEach((update: any) => {
      const componentData = this.componentRefMap.get(update.nodeId);
      
      if (!componentData) {
        return;
      }
      
      // Save focus state
      const activeElement = document.activeElement;
      const hasFocus = activeElement && this.isDescendant(componentData.ref.location.nativeElement, activeElement);
      
      // Apply each property update
      let hasChanges = false;
      Object.keys(update.properties).forEach(key => {
        const newValue = update.properties[key];
        const currentValue = componentData.ref.instance[key];
        
        if (!this.deepEqual(newValue, currentValue)) {
          componentData.ref.instance[key] = newValue;
          hasChanges = true;
        }
      });
      
      if (hasChanges) {
        // Trigger change detection only for this component
        componentData.ref.changeDetectorRef.markForCheck();
        componentData.ref.changeDetectorRef.detectChanges();
        
        // Restore focus if needed
        if (hasFocus && activeElement instanceof HTMLElement) {
          setTimeout(() => {
            if (document.activeElement !== activeElement) {
              activeElement.focus();
            }
          }, 0);
        }
      }
    });
  }

}
