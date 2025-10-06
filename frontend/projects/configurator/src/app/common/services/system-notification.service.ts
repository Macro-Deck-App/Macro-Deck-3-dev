import {EventEmitter, Injectable} from '@angular/core';
import * as signalR from '@microsoft/signalr';
import {BehaviorSubject} from 'rxjs';
import {InfiniteRetryPolicy} from '../policies/infinite-retry.policy';
import {SystemNotificationModel} from '../models/system-notification.model';

@Injectable({
  providedIn: 'root'
})
export class SystemNotificationService {
  public reconnectingSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  public systemNotificationReceived: EventEmitter<SystemNotificationModel> = new EventEmitter<SystemNotificationModel>();

  private hubConnection: signalR.HubConnection | null = null;

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/api/hubs/system-notifications')
      .withAutomaticReconnect(new InfiniteRetryPolicy())
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.reconnectingSubject.next(false);
        console.log('SignalR Connection started');
      })
      .catch(err => console.log('Error establishing SignalR connection: ' + err));

    this.hubConnection.onreconnecting(() => {
      console.log('Reconnecting...');
      this.reconnectingSubject.next(true);
    });
    this.hubConnection.onreconnected(() => {
      console.log('Reconnected!');
      this.reconnectingSubject.next(false);
    });
    this.hubConnection.onclose(() => console.log('Connection closed.'));
    this.hubConnection.onclose(error => console.log('Connection closed with error: ' + error));
    this.hubConnection.on('PublishSystemNotification', (message: SystemNotificationModel) => {
      this.systemNotificationReceived.emit(message);
    });
  }
}
