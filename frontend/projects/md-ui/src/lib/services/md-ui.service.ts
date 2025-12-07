import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MdUiConnectionService, SessionMessage, SessionError } from './md-ui-connection.service';

/**
 * Service for managing MD UI views.
 * Uses the shared MdUiConnectionService for all SignalR communication.
 * 
 * This service is now session-based: each view instance gets its own sessionId,
 * and messages are multiplexed through a single SignalR connection.
 */
@Injectable({
  providedIn: 'root'
})
export class MdUiService {
  constructor(private connectionService: MdUiConnectionService) {}

  /**
   * Initialize the global SignalR connection (call this once in app initialization)
   */
  async initializeConnection(hubUrl: string = '/api/hubs/ui'): Promise<void> {
    await this.connectionService.initialize(hubUrl);
  }

  /**
   * Navigate to a view and return the session ID
   * @param viewId The ID of the view to navigate to
   * @param sessionId Optional session ID to use (if not provided, a new one will be generated)
   */
  async navigateToView(viewId: string, sessionId?: string): Promise<string> {
    return await this.connectionService.navigateToView(viewId, sessionId);
  }

  /**
   * Send an event for a specific session
   */
  async sendEvent(sessionId: string, viewId: string, eventName: string, parameters?: any): Promise<void> {
    await this.connectionService.sendEvent(sessionId, viewId, eventName, parameters);
  }

  /**
   * Reload a view session
   */
  async reloadView(sessionId: string): Promise<void> {
    await this.connectionService.reloadView(sessionId);
  }

  /**
   * Register a session (called when a view component initializes)
   */
  registerSession(sessionId: string): void {
    this.connectionService.registerSession(sessionId);
  }

  /**
   * Unregister a session (called when a view component destroys)
   */
  unregisterSession(sessionId: string): void {
    this.connectionService.unregisterSession(sessionId);
  }

  /**
   * Get view tree messages for a specific session
   */
  getViewTreeMessages$(sessionId: string): Observable<SessionMessage> {
    return this.connectionService.getViewTreeMessages$(sessionId);
  }

  /**
   * Get property update messages for a specific session
   */
  getPropertyUpdateMessages$(sessionId: string): Observable<any> {
    return this.connectionService.getPropertyUpdateMessages$(sessionId);
  }

  /**
   * Get error messages for a specific session
   */
  getErrorMessages$(sessionId: string): Observable<SessionError> {
    return this.connectionService.getErrorMessages$(sessionId);
  }

  /**
   * Check if the connection is established
   */
  isConnected(): boolean {
    return this.connectionService.isConnected();
  }

  /**
   * Get the connection state
   */
  getConnectionState() {
    return this.connectionService.getConnectionState();
  }
}
