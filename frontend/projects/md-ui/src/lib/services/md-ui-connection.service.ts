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

export interface SessionError {
  sessionId: string;
  error: string;
}

@Injectable({ providedIn: 'root' })
export class MdUiConnectionService {
  private connection: signalR.HubConnection | null = null;
  private connectionState$ = new BehaviorSubject<signalR.HubConnectionState>(signalR.HubConnectionState.Disconnected);
  private viewTreeMessages$ = new Subject<SessionMessage>();
  private propertyUpdateMessages$ = new Subject<any>();
  private errorMessages$ = new Subject<SessionError>();
  private sessions = new Set<string>();

  constructor(private linkRequestService: LinkRequestService) {}

  async initialize(hubUrl = '/api/hubs/ui'): Promise<void> {
    if (this.connection) return;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Debug)
      .build();

    this.setupConnectionHandlers();
    this.setupMessageHandlers();

    try {
      await this.connection.start();
      this.connectionState$.next(signalR.HubConnectionState.Connected);
    } catch (err) {
      console.error('[MdUiConnection] Failed to start:', err);
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      throw err;
    }
  }

  private setupConnectionHandlers() {
    if (!this.connection) return;

    this.connection.onclose((error) => {
      if (error) console.error('[MdUiConnection] Closed:', error);
      this.connectionState$.next(signalR.HubConnectionState.Disconnected);
      this.sessions.forEach(sessionId => {
        this.errorMessages$.next({ sessionId, error: error?.message || 'Connection closed' });
      });
    });

    this.connection.onreconnecting(() => {
      this.connectionState$.next(signalR.HubConnectionState.Reconnecting);
    });

    this.connection.onreconnected(async () => {
      this.connectionState$.next(signalR.HubConnectionState.Connected);
      for (const sessionId of this.sessions) {
        try {
          await this.connection!.invoke('ReloadView', sessionId);
        } catch (err) {
          console.error('[MdUiConnection] Failed to reload session:', err);
        }
      }
    });
  }

  private setupMessageHandlers() {
    if (!this.connection) return;

    this.connection.on('ReceiveViewTree', (msg: ViewTreeMessage) => {
      this.viewTreeMessages$.next({
        sessionId: msg.sessionId,
        viewTree: msg.viewTree,
        rootViewId: msg.rootViewId
      });
    });

    this.connection.on('ReceivePropertyUpdates', (batch: any) => {
      this.propertyUpdateMessages$.next(batch);
    });

    this.connection.on('ReceiveError', (msg: UiErrorMessage) => {
      console.error('[MdUiConnection] Error:', msg.message);
      this.errorMessages$.next({ sessionId: msg.sessionId, error: msg.message });
    });

    this.connection.on('PluginDisconnected', (pluginId: string) => {
      console.warn('[MdUiConnection] Plugin disconnected:', pluginId);
    });

    this.connection.on('LinkRequest', (msg: LinkRequestMessage) => this.handleLinkRequest(msg));
  }

  private async handleLinkRequest(message: LinkRequestMessage): Promise<void> {
    const response: LinkResponseMessage = {
      requestId: message.requestId,
      approved: false
    };

    try {
      response.approved = await this.linkRequestService.showLinkRequest(message);
    } catch (error) {
      console.error('[MdUiConnection] Link request error:', error);
    }

    try {
      await this.connection!.invoke('RespondToLinkRequest', response);
    } catch (err) {
      console.error('[MdUiConnection] Failed to send response:', err);
    }
  }

  registerSession(sessionId: string) { this.sessions.add(sessionId); }
  unregisterSession(sessionId: string) { this.sessions.delete(sessionId); }

  async navigateToView(viewId: string, sessionId?: string): Promise<string> {
    if (!this.isConnected()) throw new Error('Not connected');

    const id = sessionId || crypto.randomUUID();
    this.registerSession(id);

    try {
      return await this.connection!.invoke<string>('NavigateToView', viewId, id) || id;
    } catch (error) {
      this.unregisterSession(id);
      throw error;
    }
  }

  async sendEvent(sessionId: string, viewId: string, eventName: string, parameters?: any): Promise<void> {
    if (!this.isConnected()) throw new Error('Not connected');
    await this.connection!.invoke('SendEvent', { sessionId, viewId, eventName, parameters } as UiEventMessage);
  }

  async reloadView(sessionId: string): Promise<void> {
    if (!this.isConnected()) throw new Error('Not connected');
    await this.connection!.invoke('ReloadView', sessionId);
  }

  getViewTreeMessages$(sessionId: string): Observable<SessionMessage> {
    return this.viewTreeMessages$.pipe(filter(m => m.sessionId === sessionId));
  }

  getPropertyUpdateMessages$(sessionId: string): Observable<any> {
    return this.propertyUpdateMessages$.pipe(filter(m => m.sessionId === sessionId));
  }

  getErrorMessages$(sessionId: string): Observable<SessionError> {
    return this.errorMessages$.pipe(filter(m => m.sessionId === sessionId));
  }

  getConnectionState$(): Observable<signalR.HubConnectionState> {
    return this.connectionState$.asObservable();
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}
