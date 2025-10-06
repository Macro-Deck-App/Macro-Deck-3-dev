import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { ViewTreeNode, ViewTreeMessage, UiEventMessage, UiErrorMessage } from '../models';

export interface PropertyUpdate {
  nodeId: string;
  properties: { [key: string]: any };
}

export interface PropertyUpdateBatch {
  sessionId: string;
  updates: PropertyUpdate[];
}

@Injectable({
  providedIn: 'root'
})
export class MdUiService {
  private viewTreeSubject = new BehaviorSubject<ViewTreeNode | null>(null);
  private propertyUpdatesSubject = new Subject<PropertyUpdateBatch>();
  private errorSubject = new BehaviorSubject<string | null>(null);
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private hubConnection: signalR.HubConnection | null = null;

  public viewTree$: Observable<ViewTreeNode | null> = this.viewTreeSubject.asObservable();
  public propertyUpdates$: Observable<PropertyUpdateBatch> = this.propertyUpdatesSubject.asObservable();
  public error$: Observable<string | null> = this.errorSubject.asObservable();
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  /**
   * Initialize the service with an existing SignalR connection
   */
  initialize(connection: signalR.HubConnection): void {
    this.hubConnection = connection;
    this.registerHandlers();
  }

  /**
   * Set the SignalR connection (alias for initialize)
   */
  setConnection(connection: signalR.HubConnection): void {
    this.initialize(connection);
  }

  /**
   * Register SignalR message handlers
   */
  private registerHandlers(): void {
    if (!this.hubConnection) {
      return;
    }

    this.hubConnection.on('ReceiveViewTree', (message: ViewTreeMessage) => {
      this.viewTreeSubject.next(message.viewTree);
      this.errorSubject.next(null);
      this.loadingSubject.next(false);
    });
    
    this.hubConnection.on('ReceivePropertyUpdates', (batch: PropertyUpdateBatch) => {
      this.propertyUpdatesSubject.next(batch);
    });

    this.hubConnection.on('ReceiveError', (message: UiErrorMessage) => {
      this.errorSubject.next(message.message);
      this.loadingSubject.next(false);
    });

    this.hubConnection.on('PluginDisconnected', (pluginId: string) => {
      this.errorSubject.next(`Plugin ${pluginId} has disconnected`);
    });
  }

  /**
   * Navigate to a view
   */
  navigateToView(viewId: string): void {
    if (!this.hubConnection) {
      return;
    }

    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    this.hubConnection.invoke('NavigateToView', viewId)
      .catch(err => {
        this.errorSubject.next(`Failed to navigate: ${err.message}`);
        this.loadingSubject.next(false);
      });
  }

  /**
   * Send an event to the backend
   */
  sendEvent(viewId: string, eventName: string, parameters?: any): void {
    if (!this.hubConnection) {
      return;
    }

    const eventMessage: UiEventMessage = {
      sessionId: this.hubConnection.connectionId || '',
      viewId,
      eventName,
      parameters
    };

    this.hubConnection.invoke('SendEvent', eventMessage)
      .catch(err => {
        this.errorSubject.next(`Failed to send event: ${err.message}`);
      });
  }

  /**
   * Reload the current view
   */
  reloadView(): void {
    if (!this.hubConnection) {
      return;
    }

    this.loadingSubject.next(true);

    this.hubConnection.invoke('ReloadView')
      .catch(err => {
        this.errorSubject.next(`Failed to reload: ${err.message}`);
        this.loadingSubject.next(false);
      });
  }

  /**
   * Clear error
   */
  clearError(): void {
    this.errorSubject.next(null);
  }

  /**
   * Get current view tree
   */
  getCurrentViewTree(): ViewTreeNode | null {
    return this.viewTreeSubject.value;
  }
}
