import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MdUiConnectionService, SessionMessage, SessionError } from './md-ui-connection.service';

@Injectable({ providedIn: 'root' })
export class MdUiService {
  constructor(private connectionService: MdUiConnectionService) {}

  async initializeConnection(hubUrl = '/api/hubs/ui'): Promise<void> {
    await this.connectionService.initialize(hubUrl);
  }

  async navigateToView(viewId: string, sessionId?: string): Promise<string> {
    return this.connectionService.navigateToView(viewId, sessionId);
  }

  async sendEvent(sessionId: string, viewId: string, eventName: string, parameters?: any): Promise<void> {
    await this.connectionService.sendEvent(sessionId, viewId, eventName, parameters);
  }

  async reloadView(sessionId: string): Promise<void> {
    await this.connectionService.reloadView(sessionId);
  }

  registerSession(sessionId: string) { this.connectionService.registerSession(sessionId); }
  unregisterSession(sessionId: string) { this.connectionService.unregisterSession(sessionId); }

  getViewTreeMessages$(sessionId: string): Observable<SessionMessage> {
    return this.connectionService.getViewTreeMessages$(sessionId);
  }

  getPropertyUpdateMessages$(sessionId: string): Observable<any> {
    return this.connectionService.getPropertyUpdateMessages$(sessionId);
  }

  getErrorMessages$(sessionId: string): Observable<SessionError> {
    return this.connectionService.getErrorMessages$(sessionId);
  }

  getConnectionState$() { return this.connectionService.getConnectionState$(); }
  isConnected() { return this.connectionService.isConnected(); }
}
