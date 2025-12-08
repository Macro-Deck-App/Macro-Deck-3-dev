import { Component, Input, ViewChild, ViewContainerRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../../models';

@Component({
  selector: 'md-container',
  template: `
    <div 
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.background-color]="backgroundColor"
      [style.width.px]="width"
      [style.height.px]="height"
      [style.border-radius]="borderRadiusStyle"
      [style.border]="borderStyle"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle"
      [style.display]="alignment ? 'flex' : undefined"
      [style.justify-content]="getJustifyContent()"
      [style.align-items]="getAlignItems()">
      <ng-container #childContainer></ng-container>
    </div>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdContainerComponent {
  @Input() backgroundColor?: string;
  @Input() width?: number;
  @Input() height?: number;
  @Input() borderRadius?: any;
  @Input() border?: any;
  @Input() alignment?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  @ViewChild('childContainer', { read: ViewContainerRef, static: true })
  childContainer!: ViewContainerRef;

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }

  get borderRadiusStyle(): string | undefined {
    if (!this.borderRadius) return undefined;
    if (typeof this.borderRadius === 'number') {
      return `${this.borderRadius}px`;
    }
    return `${this.borderRadius.topLeft}px ${this.borderRadius.topRight}px ${this.borderRadius.bottomRight}px ${this.borderRadius.bottomLeft}px`;
  }

  get borderStyle(): string | undefined {
    if (!this.border) return undefined;
    return `${this.border.width}px ${this.border.style || 'solid'} ${this.border.color}`;
  }

  getJustifyContent(): string | undefined {
    if (!this.alignment) return undefined;
    
    switch (this.alignment.toLowerCase()) {
      case 'topleft':
      case 'centerleft':
      case 'bottomleft':
        return 'flex-start';
      case 'topcenter':
      case 'center':
      case 'bottomcenter':
        return 'center';
      case 'topright':
      case 'centerright':
      case 'bottomright':
        return 'flex-end';
      default:
        return undefined;
    }
  }

  getAlignItems(): string | undefined {
    if (!this.alignment) return undefined;
    
    switch (this.alignment.toLowerCase()) {
      case 'topleft':
      case 'topcenter':
      case 'topright':
        return 'flex-start';
      case 'centerleft':
      case 'center':
      case 'centerright':
        return 'center';
      case 'bottomleft':
      case 'bottomcenter':
      case 'bottomright':
        return 'flex-end';
      default:
        return undefined;
    }
  }
}
