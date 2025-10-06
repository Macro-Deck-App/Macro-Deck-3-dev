import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {FolderService} from './common/services/folder.service';
import {SystemNotificationService} from './common/services/system-notification.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {ConnectingComponent} from './connecting/connecting.component';
import {WidgetService} from './common/services/widget.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ConnectingComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'Macro Deck';

  public reconnecting = false;

  private destroyRef = inject(DestroyRef);

  constructor(private folderService: FolderService,
              private widgetService: WidgetService,
              private systemNotificationsService: SystemNotificationService) {
  }

  public async ngOnInit() {
    this.systemNotificationsService.reconnectingSubject
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (value) => {
          this.reconnecting = value;
        }
      })

    await this.folderService.updateFolders();
    await this.widgetService.updateWidgets();
    this.systemNotificationsService.startConnection();
  }
}
