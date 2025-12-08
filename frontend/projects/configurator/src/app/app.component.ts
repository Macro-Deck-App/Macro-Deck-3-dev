import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {FolderService} from './common/services/folder.service';
import {SystemNotificationService} from './common/services/system-notification.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {ConnectingComponent} from './connecting/connecting.component';
import {WidgetService} from './common/services/widget.service';
import { LinkRequestService as MdUiLinkRequestService } from 'md-ui';
import { LinkRequestService as ConfiguratorLinkRequestService } from './common/services/link-request.service';

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
              private systemNotificationsService: SystemNotificationService,
              private mdUiLinkRequestService: MdUiLinkRequestService,
              private configuratorLinkRequestService: ConfiguratorLinkRequestService) {
  }

  public async ngOnInit() {
    // Set up link request provider for md-ui library
    this.mdUiLinkRequestService.setProvider((request) => {
      return this.configuratorLinkRequestService.showLinkRequest(request);
    });

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
