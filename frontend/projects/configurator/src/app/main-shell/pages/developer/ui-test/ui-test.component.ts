import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ViewSidebarComponent, ViewMetadata } from './view-sidebar/view-sidebar.component';
import { ViewContentComponent } from './view-content/view-content.component';
import { MdUiViewRegistryService } from 'md-ui';
import { Subscription } from 'rxjs';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

@Component({
  selector: 'app-ui-test',
  imports: [CommonModule, ViewSidebarComponent, ViewContentComponent],
  providers: [MdUiViewRegistryService],
  templateUrl: './ui-test.component.html',
  styleUrl: './ui-test.component.scss'
})
export class UiTestComponent implements OnInit, OnDestroy {
  views: ViewMetadata[] = [];
  selectedViewId?: string;
  hubConnection?: HubConnection;
  isLoading = true;
  errorMessage?: string;
  private viewsSubscription?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private viewRegistryService: MdUiViewRegistryService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.initializeSignalR();
    await this.loadRegisteredViews();

    // Subscribe to route params to load the view from URL
    this.route.params.subscribe(params => {
      const viewId = params['viewId'];
      if (viewId && this.views.some(v => v.viewId === viewId)) {
        this.selectView(viewId);
      }
    });

    // Subscribe to view registry changes
    this.viewsSubscription = this.viewRegistryService.views$.subscribe((views: any[]) => {
      this.views = views;
    });
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
    if (this.viewsSubscription) {
      this.viewsSubscription.unsubscribe();
    }
  }

  private async initializeSignalR(): Promise<void> {
    try {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl('/api/hubs/ui')
        .withAutomaticReconnect()
        .build();

      // Handle errors from the server
      this.hubConnection.on('ReceiveError', (errorMessage: any) => {
        console.error('[UiTestComponent] Received error from server:', errorMessage);
        this.errorMessage = errorMessage.message || 'An error occurred';
      });

      await this.hubConnection.start();

      // Subscribe to view registry changes
      this.viewRegistryService.subscribeToChanges(this.hubConnection);
    } catch (error) {
      console.error('[UiTestComponent] SignalR connection error:', error);
      this.errorMessage = 'Failed to connect to UI Hub. Please refresh the page.';
    }
  }

  private async loadRegisteredViews(): Promise<void> {
    try {
      this.isLoading = true;
      await this.viewRegistryService.refreshViews();
    } catch (error) {
      console.error('[UiTestComponent] Error loading views:', error);
      this.errorMessage = 'Failed to load registered views';
    } finally {
      this.isLoading = false;
    }
  }

  onViewSelected(viewId: string): void {
    this.selectView(viewId);
    // Update URL without reloading the page
    this.router.navigate(['/developer/ui-test', viewId]);
  }

  private selectView(viewId: string): void {
    // Simply set the selected view ID
    // The MdUiViewComponent will handle the SignalR session and navigation
    this.selectedViewId = viewId;
    this.errorMessage = undefined;
  }

  onReloadView(): void {
    // Force reload by temporarily clearing and resetting the selectedViewId
    // This will cause MdUiViewComponent to recreate with a new session
    const currentViewId = this.selectedViewId;
    this.selectedViewId = undefined;
    this.errorMessage = undefined;
    
    // Use setTimeout to ensure Angular detects the change
    setTimeout(() => {
      this.selectedViewId = currentViewId;
    }, 0);
  }
}
