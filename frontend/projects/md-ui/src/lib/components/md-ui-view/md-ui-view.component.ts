import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdUiRendererComponent } from '../md-ui-renderer.component';
import { MdUiService } from '../../services/md-ui.service';
import { ViewTreeNode } from '../../models/view-tree-node.model';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';

/**
 * Wrapper component that simplifies using the MD UI Framework.
 * Just provide a viewId and it handles the connection, navigation and rendering.
 * 
 * Usage:
 * <md-ui-view viewId="server.TestCounterView"></md-ui-view>
 */
@Component({
  selector: 'md-ui-view',
  standalone: true,
  imports: [CommonModule, MdUiRendererComponent],
  providers: [MdUiService],
  template: `
    @if (currentTree) {
      <md-ui-renderer [viewTree]="currentTree"></md-ui-renderer>
    } @else if (isConnecting) {
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
export class MdUiViewComponent implements OnInit, OnDestroy {
  @Input() viewId!: string;

  currentTree: ViewTreeNode | null = null;
  isConnecting = false;
  error: string | null = null;
  
  private connection: HubConnection | null = null;

  constructor(private mdUiService: MdUiService) {}

  async ngOnInit() {
    if (!this.viewId) {
      this.error = 'No viewId provided';
      return;
    }

    await this.connectAndNavigate();
  }

  async ngOnDestroy() {
    if (this.connection && this.connection.state === HubConnectionState.Connected) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  async reload() {
    this.error = null;
    this.currentTree = null;
    
    if (this.connection && this.connection.state === HubConnectionState.Connected) {
      await this.connection.stop();
    }
    
    await this.connectAndNavigate();
  }

  private async connectAndNavigate() {
    try {
      this.isConnecting = true;
      this.error = null;

      // Build SignalR connection
      this.connection = new HubConnectionBuilder()
        .withUrl('/api/hubs/ui')
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      // Setup MdUiService to use this connection for sending events
      this.mdUiService.setConnection(this.connection);

      // Listen for view tree updates
      this.connection.on('ReceiveViewTree', (data: { sessionId: string, viewTree: ViewTreeNode, rootViewId: string }) => {
        console.log('[MdUiView] Received view tree:', data);
        this.currentTree = data.viewTree;
        this.isConnecting = false;
      });

      // Handle disconnection
      this.connection.onclose((error) => {
        console.error('[MdUiView] Connection closed:', error);
        if (error) {
          this.error = `Connection lost: ${error.message || error}`;
        }
        this.isConnecting = false;
      });

      // Connect to hub
      console.log('[MdUiView] Connecting to UI hub...');
      await this.connection.start();
      console.log('[MdUiView] Connected to UI hub');
      
      // Navigate to view
      console.log('[MdUiView] Navigating to view:', this.viewId);
      await this.connection.invoke('NavigateToView', this.viewId);
      
    } catch (err: any) {
      console.error('[MdUiView] Error connecting or navigating:', err);
      this.error = `Failed to load view: ${err.message || err}`;
      this.isConnecting = false;
    }
  }
}
