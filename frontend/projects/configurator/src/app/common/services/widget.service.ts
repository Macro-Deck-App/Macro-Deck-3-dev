import { Injectable } from '@angular/core';
import {BehaviorSubject, firstValueFrom} from 'rxjs';
import {WidgetModel} from '../models/widget.model';
import {HttpClient} from '@angular/common/http';
import {SystemNotificationService} from './system-notification.service';
import {FolderService} from './folder.service';
import {SystemNotificationType} from '../enums/system-notification-type.enum';
import {CreateWidgetModel} from '../models/create-widget.model';

@Injectable({
  providedIn: 'root'
})
export class WidgetService {
  public widgetsSubject: BehaviorSubject<WidgetModel[]> = new BehaviorSubject<WidgetModel[]>([]);

  constructor(
    private http: HttpClient,
    private folderService: FolderService,
    systemNotificationService: SystemNotificationService
  ) {
    systemNotificationService.reconnectingSubject
      .subscribe(async reconnecting => {
        if (!reconnecting) {
          await this.updateWidgets();
        }
      });
    systemNotificationService.systemNotificationReceived
      .subscribe(async x => {
        if (x.type === SystemNotificationType.WidgetCreated
          || x.type === SystemNotificationType.WidgetDeleted
          || x.type === SystemNotificationType.WidgetUpdated) {
          await this.updateWidgets();
        }
      });
    folderService.selectedFolderSubject.subscribe(async () => {
      await this.updateWidgets();
    });
  }

  async updateWidgets() {
    const folderId = this.folderService.selectedFolderSubject.value?.id;
    if (!folderId) {
      return;
    }

    const widgets = await firstValueFrom(this.http.get<WidgetModel[]>(`/api/folders/${folderId}/widgets`));
    this.widgetsSubject.next(widgets);
  }

  async updateWidget(widget: WidgetModel) {

  }

  getWidget(row: number, column: number): WidgetModel | null {
    return this.widgetsSubject.value.find(x => x.row === row && x.column === column) ?? null;
  }

  createWidget(createWidget: CreateWidgetModel) {
    const folderId = this.folderService.selectedFolderSubject.value?.id;
    if (!folderId) {
      return;
    }

    return firstValueFrom(this.http.post(`/api/folders/${folderId}/widgets`, createWidget));
  }

  deleteWidget(id: string) {
    return firstValueFrom(this.http.delete(`/api/widgets/${id}`));
  }
}
