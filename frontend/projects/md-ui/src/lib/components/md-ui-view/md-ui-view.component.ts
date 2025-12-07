import { Component, Input, OnInit, OnDestroy, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdUiRendererComponent } from '../md-ui-renderer.component';
import { MdUiService } from '../../services/md-ui.service';
import { ViewTreeNode } from '../../models/view-tree-node.model';
import { Subscription } from 'rxjs';

/**
 * Wrapper component that simplifies using the MD UI Framework.
 * Just provide a viewId and it handles the session management, navigation and rendering.
 * 
 * Uses the shared SignalR connection from MdUiConnectionService, so multiple instances
 * of this component don't create multiple connections.
 * 
 * Usage:
 * <md-ui-view viewId="server.TestCounterView"></md-ui-view>
 */
@Component({
  selector: 'md-ui-view',
  standalone: true,
  imports: [CommonModule, MdUiRendererComponent],
  template: `
    @if (currentTree) {
      <md-ui-renderer [viewTree]="currentTree" [sessionId]="sessionId"></md-ui-renderer>
    } @else if (isLoading) {
      <div class="d-flex justify-content-center align-items-center p-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
    } @else if (error) {
      <div class="alert alert-danger m-3" role="alert">
        <h4 class="alert-heading">Error</h4>
        <p>{{ error }}</p>
        <hr>
        <button class="btn btn-danger" (click)="reload()">Reload</button>
      </div>
    }
  `
})
export class MdUiViewComponent implements OnInit, OnDestroy, OnChanges {
  @Input() viewId!: string;
  @ViewChild(MdUiRendererComponent) renderer?: MdUiRendererComponent;

  currentTree: ViewTreeNode | null = null;
  isLoading = false;
  error: string | null = null;
  sessionId: string | null = null;
  
  private subscriptions: Subscription[] = [];

  constructor(private mdUiService: MdUiService) {}

  async ngOnInit() {
    if (!this.viewId) {
      this.error = 'No viewId provided';
      return;
    }

    await this.loadView();
  }

  async ngOnChanges(changes: SimpleChanges) {
    // If viewId changes (and it's not the first change), reload the view
    if (changes['viewId'] && !changes['viewId'].firstChange) {
      await this.reload();
    }
  }

  async ngOnDestroy() {
    // Unsubscribe from all observables
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.subscriptions = [];

    // Unregister session
    if (this.sessionId) {
      this.mdUiService.unregisterSession(this.sessionId);
      this.sessionId = null;
    }
  }

  async reload() {
    this.error = null;
    this.currentTree = null;
    
    // Unregister old session
    if (this.sessionId) {
      this.mdUiService.unregisterSession(this.sessionId);
      this.sessionId = null;
    }
    
    // Unsubscribe from old subscriptions
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.subscriptions = [];
    
    await this.loadView();
  }

  private async loadView() {
    try {
      this.isLoading = true;
      this.error = null;

      // Ensure connection is initialized
      if (!this.mdUiService.isConnected()) {
        await this.mdUiService.initializeConnection();
      }

      // Create a session ID upfront (we'll use it for pre-subscription)
      const tempSessionId = this.generateSessionId();
      
      // Subscribe to view tree updates BEFORE navigating (to avoid race condition)
      const viewTreeSub = this.mdUiService.getViewTreeMessages$(tempSessionId).subscribe({
        next: (message) => {
          this.currentTree = message.viewTree;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('[MdUiView] Error receiving view tree:', err);
          this.error = `Failed to receive view tree: ${err.message || err}`;
          this.isLoading = false;
        }
      });
      this.subscriptions.push(viewTreeSub);

      // Navigate to view with pre-registered session ID
      this.sessionId = await this.mdUiService.navigateToView(this.viewId, tempSessionId);

      // Subscribe to error messages for this session
      const errorSub = this.mdUiService.getErrorMessages$(this.sessionId).subscribe({
        next: (errorMsg) => {
          console.error('[MdUiView] Received error for session:', this.sessionId, errorMsg);
          this.error = errorMsg.error;
          this.isLoading = false;
        }
      });
      this.subscriptions.push(errorSub);

      // Subscribe to property updates for this session
      const propertyUpdateSub = this.mdUiService.getPropertyUpdateMessages$(this.sessionId).subscribe({
        next: (batch) => {
          if (this.renderer) {
            this.renderer.applyPropertyUpdates(batch);
          } else {
          }
        },
        error: (err) => {
          console.error('[MdUiView] Error receiving property updates:', err);
        }
      });
      this.subscriptions.push(propertyUpdateSub);

    } catch (err: any) {
      console.error('[MdUiView] Error loading view:', err);
      this.error = `Failed to load view: ${err.message || err}`;
      this.isLoading = false;
    }
  }

  /**
   * Generate a unique session ID for this view instance
   */
  private generateSessionId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }
}
