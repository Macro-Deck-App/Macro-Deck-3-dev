import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MdUiService, MdUiRendererComponent } from 'md-ui';
import * as signalR from '@microsoft/signalr';
import { Subject, takeUntil } from 'rxjs';

/**
 * Zentraler UI Container der dynamisch Views vom Server lädt
 * 
 * Usage:
 * - Route: /ui/:viewId
 * - Beispiel: /ui/server.TestCounterView
 * - Beispiel: /ui/app.macrodeck.exampleplugin.ConfigView
 */
@Component({
  selector: 'app-ui-container',
  standalone: true,
  imports: [CommonModule, MdUiRendererComponent],
  template: `
    <div class="ui-container">
      <!-- Loading State -->
      @if (loading$ | async) {
        <div class="ui-loading">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
          <p class="mt-3">Loading view...</p>
        </div>
      }
      
      <!-- Error State -->
      @if (error$ | async; as error) {
        <div class="ui-error">
          <div class="alert alert-danger" role="alert">
            <h4 class="alert-heading">
              <i class="mdi mdi-alert-circle"></i> Error
            </h4>
            <p>{{ error }}</p>
            <hr>
            <div class="d-flex gap-2">
              <button class="btn btn-primary" (click)="reload()">
                <i class="mdi mdi-reload"></i> Reload
              </button>
              <button class="btn btn-outline-secondary" (click)="goBack()">
                <i class="mdi mdi-arrow-left"></i> Go Back
              </button>
            </div>
          </div>
        </div>
      }
      
      <!-- UI Renderer -->
      @if (!(loading$ | async) && !(error$ | async)) {
        <div class="ui-content">
          <md-ui-renderer 
            [viewTree]="viewTree$ | async">
          </md-ui-renderer>
        </div>
      }
    </div>
  `,
  styles: [`
    .ui-container {
      width: 100%;
      min-height: 400px;
    }
    
    .ui-loading {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 60px 20px;
      color: #6c757d;
    }
    
    .ui-error {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }
    
    .ui-content {
      width: 100%;
    }
  `]
})
export class UiContainerComponent implements OnInit, OnDestroy {
  private mdUiService: MdUiService = inject(MdUiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private hubConnection: signalR.HubConnection | null = null;
  private destroy$ = new Subject<void>();
  
  viewTree$ = this.mdUiService.viewTree$;
  error$ = this.mdUiService.error$;
  loading$ = this.mdUiService.loading$;
  
  currentViewId: string | null = null;

  async ngOnInit(): Promise<void> {
    console.log('[UiContainer] Initialized');
    
    // Connect to SignalR if not already connected
    await this.ensureConnection();
    
    // Listen to route changes
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const viewId = params['viewId'];
        if (viewId) {
          this.loadView(viewId);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    
    // Note: We don't stop the hub connection here because it might be shared
    // If you want to stop it, uncomment:
    // if (this.hubConnection) {
    //   this.hubConnection.stop();
    // }
  }

  private async ensureConnection(): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    try {
      console.log('[UiContainer] Connecting to UI Hub...');
      
      // Create SignalR connection
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('/api/hubs/ui')
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Initialize MdUiService with connection
      this.mdUiService.initialize(this.hubConnection);

      // Handle reconnection
      this.hubConnection.onreconnected(() => {
        console.log('[UiContainer] Reconnected, reloading view...');
        if (this.currentViewId) {
          this.loadView(this.currentViewId);
        }
      });

      // Start connection
      await this.hubConnection.start();
      console.log('[UiContainer] Connected to UI Hub');
      
    } catch (error) {
      console.error('[UiContainer] Connection error:', error);
      throw error;
    }
  }

  private loadView(viewId: string): void {
    console.log('[UiContainer] Loading view:', viewId);
    this.currentViewId = viewId;
    this.mdUiService.navigateToView(viewId);
  }

  reload(): void {
    console.log('[UiContainer] Reloading view...');
    this.mdUiService.clearError();
    if (this.currentViewId) {
      this.mdUiService.navigateToView(this.currentViewId);
    } else {
      this.mdUiService.reloadView();
    }
  }

  goBack(): void {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
