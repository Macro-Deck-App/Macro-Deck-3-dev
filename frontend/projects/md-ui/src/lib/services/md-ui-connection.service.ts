import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject, filter } from 'rxjs';
import { ViewTreeNode, ViewTreeMessage, UiEventMessage, UiErrorMessage, LinkRequestMessage, LinkResponseMessage } from '../models';
import { LinkRequestService } from './link-request.service';

export interface SessionMessage {
  sessionId: string;
  viewTree: ViewTreeNode;
  rootViewId: string;
}

export interface SessionPropertyUpdate {
  sessionId: string;
  nodeId: string;
  properties: { [key: string]: any };
}

export interface SessionError {
  sessionId: string;
  error: string;
}

/**
 * Central SignalR connection manager for MD UI.
 * Manages a single SignalR connection and multiplexes messages to different view sessions.
 */
@Injectable({
  providedIn: 'root'
})
export class MdUiConnectionService {
  private connection: signalR.HubConnection | null = null;
  private connectionState$ = new BehaviorSubject<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
  private viewTreeMessages$ = new Subject<SessionMessage>();
  private propertyUpdateMessages$ = new Subject<any>();
  private errorMessages$ = new Subject<SessionError>();
  private sessions = new Set<string>();

  constructor(private linkRequestService: LinkRequestService) {}

  /**
   * Initialize the connection (called once globally)
   */
  async initialize(hubUrl: string = '/api/hubs/ui'): Promise<void> {
    if (this.connection) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Debug)
      .build();

    this.registerHandlers();

    // Connection state management
    this.connection.onclose((error) => {
      if (error) {
        console.error('[MdUiConnection] Connection closed:', error);
      }
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);

      // Notify all sessions about the error
      this.sessions.forEach(sessionId => {
        this.errorMessages$.next({
          sessionId,
          error: error?.message || 'Connection closed'
        });
      });
    });

    this.connection.onreconnecting(() => {
      this.connectionState$.next(signalR.HubConnectionState.Reconnecting);
    });

    this.connection.onreconnected(async (connectionId) => {
      this.connectionState$.next(signalR.HubConnectionState.Connected);

      // Reload all active sessions
      for (const sessionId of this.sessions) {
        try {
          await this.connection!.invoke('ReloadView', sessionId);
        } catch (err) {
          console.error('[MdUiConnection] Failed to reload session:', err);
        }
      }
    });

    // Start connection
    try {
      await this.connection.start();
      this.connectionState$.next(signalR.HubConnectionState.Connected);
    } catch (err) {
      console.error('[MdUiConnection] Failed to start connection:', err);
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      throw err;
    }
  }

  /**
   * Register SignalR message handlers
   */
  private registerHandlers(): void {
    if (!this.connection) {
      return;
    }

    // Handle view tree updates
    this.connection.on('ReceiveViewTree', (message: ViewTreeMessage) => {
      this.viewTreeMessages$.next({
        sessionId: message.sessionId,
        viewTree: message.viewTree,
        rootViewId: message.rootViewId
      });
    });

    // Handle property updates
    this.connection.on('ReceivePropertyUpdates', (batch: any) => {
      this.propertyUpdateMessages$.next(batch);
    });

    // Handle errors
    this.connection.on('ReceiveError', (message: UiErrorMessage) => {
      console.error('[MdUiConnection] Error from server:', message.message);
      this.errorMessages$.next({
        sessionId: message.sessionId,
        error: message.message
      });
    });

    // Handle plugin disconnections
    this.connection.on('PluginDisconnected', (pluginId: string) => {
      console.warn('[MdUiConnection] Plugin disconnected:', pluginId);
    });

    // Handle link requests
    this.connection.on('LinkRequest', async (message: LinkRequestMessage) => {
      await this.handleLinkRequest(message);
    });
  }

  /**
   * Handle link request message
   */
  private async handleLinkRequest(message: LinkRequestMessage): Promise<void> {
    try {
      const approved = await this.linkRequestService.showLinkRequest(message);

      const response: LinkResponseMessage = {
        requestId: message.requestId,
        approved: approved
      };

      await this.connection!.invoke('RespondToLinkRequest', response);
    } catch (error) {
      console.error('[MdUiConnection] Error handling link request:', error);

      const response: LinkResponseMessage = {
        requestId: message.requestId,
        approved: false
      };

      try {
        await this.connection!.invoke('RespondToLinkRequest', response);
      } catch (err) {
        console.error('[MdUiConnection] Failed to send error response:', err);
      }
    }
  }

  /**
   * Register a new view session
   */
  registerSession(sessionId: string): void {
    this.sessions.add(sessionId);
  }

  /**
   * Unregister a view session
   */
  unregisterSession(sessionId: string): void {
    this.sessions.delete(sessionId);
  }

  /**
   * Navigate to a view (creates a new session)
   * Returns a Promise that resolves with the sessionId
   */
  async navigateToView(viewId: string, sessionId?: string): Promise<string> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('Not connected to UI hub');
    }

    // Use provided session ID or generate a new one
    const actualSessionId = sessionId || this.generateSessionId();


    // Register the session before navigating
    this.registerSession(actualSessionId);

    try {
      // Invoke NavigateToView with the session ID and get the returned session ID from the server
      const returnedSessionId = await this.connection.invoke<string>('NavigateToView', viewId, actualSessionId);

      // Use the session ID returned from the server (should be the same as actualSessionId)
      return returnedSessionId || actualSessionId;
    } catch (error) {
      console.error('[MdUiConnection] Navigation failed:', error);
      // Unregister session on failure
      this.unregisterSession(actualSessionId);
      throw error;
    }
  }

  /**
   * Generate a unique session ID
   */
  private generateSessionId(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }

  /**
   * Send an event for a specific session
   */
  async sendEvent(sessionId: string, viewId: string, eventName: string, parameters?: any): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('Not connected to UI hub');
    }

    const eventMessage: UiEventMessage = {
      sessionId,
      viewId,
      eventName,
      parameters
    };

    await this.connection.invoke('SendEvent', eventMessage);
  }

  /**
   * Reload a view session
   */
  async reloadView(sessionId: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('Not connected to UI hub');
    }

    await this.connection.invoke('ReloadView', sessionId);
  }

  /**
   * Get view tree messages for a specific session
   */
  getViewTreeMessages$(sessionId: string): Observable<SessionMessage> {
    return this.viewTreeMessages$.pipe(
      filter(msg => msg.sessionId === sessionId)
    );
  }

  /**
   * Get property update messages for a specific session
   */
  getPropertyUpdateMessages$(sessionId: string): Observable<any> {
    return this.propertyUpdateMessages$.pipe(
      filter(msg => msg.sessionId === sessionId)
    );
  }

  /**
   * Get error messages for a specific session
   */
  getErrorMessages$(sessionId: string): Observable<SessionError> {
    return this.errorMessages$.pipe(
      filter(msg => msg.sessionId === sessionId)
    );
  }

  /**
   * Get connection state observable
   */
  getConnectionState$(): Observable<signalR.HubConnectionState> {
    return this.connectionState$.asObservable();
  }

  /**
   * Get current connection state
   */
  getConnectionState(): signalR.HubConnectionState {
    return this.connectionState$.value;
  }

  /**
   * Check if connected
   */
  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  /**
   * Get connection ID
   */
  getConnectionId(): string | null {
    return this.connection?.connectionId || null;
  }
}
