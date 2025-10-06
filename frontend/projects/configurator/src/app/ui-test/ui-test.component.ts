import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdUiService, MdUiRendererComponent } from 'md-ui';
import * as signalR from '@microsoft/signalr';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-ui-test',
  standalone: true,
  imports: [CommonModule, MdUiRendererComponent],
  template: `
    <div class="ui-test-page">
      <div class="header">
        <h1>MacroDeck UI Framework Test</h1>
        <p>Testing the UI framework with a live view from the server</p>
      </div>
      
      <div class="controls">
        <button (click)="connectAndLoad()" class="btn btn-primary">
          {{ (loading$ | async) ? 'Loading...' : 'Load Test View' }}
        </button>
        <button (click)="reload()" class="btn btn-secondary" [disabled]="!isConnected">
          Reload
        </button>
      </div>
      
      @if (error$ | async; as error) {
        <div class="alert alert-danger">
          <strong>Error:</strong> {{ error }}
        </div>
      }
      
      <div class="ui-container">
        <md-ui-renderer 
          [viewTree]="viewTree$ | async">
        </md-ui-renderer>
      </div>
    </div>
  `,
  styles: [`
    .ui-test-page {
      padding: 20px;
      max-width: 1200px;
      margin: 0 auto;
    }
    
    .header {
      margin-bottom: 30px;
    }
    
    .header h1 {
      color: #333;
      margin-bottom: 10px;
    }
    
    .header p {
      color: #666;
      font-size: 16px;
    }
    
    .controls {
      margin-bottom: 20px;
      display: flex;
      gap: 10px;
    }
    
    .btn {
      padding: 10px 20px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
    }
    
    .btn-primary {
      background-color: #007bff;
      color: white;
    }
    
    .btn-primary:hover {
      background-color: #0056b3;
    }
    
    .btn-secondary {
      background-color: #6c757d;
      color: white;
    }
    
    .btn-secondary:hover {
      background-color: #545b62;
    }
    
    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
    
    .alert {
      padding: 15px;
      border-radius: 4px;
      margin-bottom: 20px;
    }
    
    .alert-danger {
      background-color: #f8d7da;
      border: 1px solid #f5c6cb;
      color: #721c24;
    }
    
    .ui-container {
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
      padding: 20px;
      min-height: 400px;
    }
  `]
})
export class UiTestComponent implements OnInit, OnDestroy {
  private mdUiService: MdUiService = inject(MdUiService);
  private hubConnection: signalR.HubConnection | null = null;
  private destroy$ = new Subject<void>();
  
  viewTree$ = this.mdUiService.viewTree$;
  error$ = this.mdUiService.error$;
  loading$ = this.mdUiService.loading$;
  
  isConnected = false;

  constructor() {}

  ngOnInit(): void {
    console.log('[UiTestComponent] Initialized');
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  async connectAndLoad(): Promise<void> {
    try {
      console.log('[UiTestComponent] Connecting to UI Hub...');
      
      // Create SignalR connection
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('/api/hubs/ui')
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Initialize MdUiService with connection
      this.mdUiService.initialize(this.hubConnection);

      // Start connection
      await this.hubConnection.start();
      console.log('[UiTestComponent] Connected to UI Hub');
      
      this.isConnected = true;

      // Navigate to test view
      console.log('[UiTestComponent] Navigating to TestCounterView...');
      this.mdUiService.navigateToView('server.TestCounterView');
      
    } catch (error) {
      console.error('[UiTestComponent] Connection error:', error);
      alert(`Failed to connect: ${error}`);
    }
  }

  reload(): void {
    console.log('[UiTestComponent] Reloading view...');
    this.mdUiService.reloadView();
  }
}
