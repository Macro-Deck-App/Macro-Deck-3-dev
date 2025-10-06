import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, firstValueFrom} from 'rxjs';
import {FolderWithChildsModel} from '../models/folder-with-childs.model';
import {FolderModel} from '../models/folder.model';
import {SystemNotificationService} from './system-notification.service';
import {SystemNotificationType} from '../enums/system-notification-type.enum';
import {CreateFolderModel} from '../models/create-folder.model';
import {flattenFolders} from '../utils/folder-utils';

@Injectable({
  providedIn: 'root'
})
export class FolderService {
  public foldersSubject: BehaviorSubject<FolderWithChildsModel[] | null> = new BehaviorSubject<FolderWithChildsModel[] | null>(null);
  public selectedFolderSubject: BehaviorSubject<FolderModel | null> = new BehaviorSubject<FolderModel | null>(null);

  constructor(
    private http: HttpClient,
    systemNotificationService: SystemNotificationService
  ) {
    systemNotificationService.reconnectingSubject
      .subscribe(async reconnecting => {
        if (!reconnecting) {
          await this.updateFolders();
        }
      });
    systemNotificationService.systemNotificationReceived
      .subscribe(async notification => {
        if (notification.type === SystemNotificationType.FolderCreated
          || notification.type === SystemNotificationType.FolderDeleted
          || notification.type === SystemNotificationType.FolderUpdated) {
          await this.updateFolders();

          if (this.selectedFolderSubject.value) {
            const selectedFolder = this.findFolderById(notification.parameters as number);
            if (selectedFolder) {
              this.selectFolder(selectedFolder);
            }
          }
        }
      });
  }

  public selectFolder(folder: FolderModel | null) {
    this.selectedFolderSubject.next(folder);
  }

  public async updateFolders() {
    const folders = await firstValueFrom(this.http.get<FolderWithChildsModel[]>('/api/folders'));
    this.foldersSubject.next(folders);

    if (!this.selectedFolderSubject.value && folders[0]) {
      this.selectFolder(folders[0]);
    }
  }

  public findFolderById(id: number): FolderModel | null {
    return flattenFolders(this.foldersSubject.value ?? []).find(x => x.id === id) ?? null;
  }

  public createFolder(displayName: string, parentFolderId: number | null | undefined): Promise<FolderModel> {
    const createFolder: CreateFolderModel = {
      displayName: displayName,
      parentFolderId: parentFolderId
    };

    return firstValueFrom(this.http.post<FolderModel>('/api/folders', createFolder));
  }

  public async updateFolder(existingFolder: FolderModel): Promise<void> {
    await firstValueFrom(this.http.put(`/api/folders/${existingFolder.id}`, existingFolder));
  }

  public async deleteFolder(id: number) {
    await firstValueFrom(this.http.delete(`/api/folders/${id}`));
  }

  public async addRowToCurrentFolder() {
    if (!this.selectedFolderSubject.value) {
      return;
    }

    this.selectedFolderSubject.value.rows++;
    await this.updateFolder(this.selectedFolderSubject.value);
  }

  public async removeRowFromCurrentFolder() {
    if (!this.selectedFolderSubject.value) {
      return;
    }

    this.selectedFolderSubject.value.rows--;
    await this.updateFolder(this.selectedFolderSubject.value);
  }

  async addColumnToCurrentFolder() {
    if (!this.selectedFolderSubject.value) {
      return;
    }

    this.selectedFolderSubject.value.columns++;
    await this.updateFolder(this.selectedFolderSubject.value);
  }

  async removeColumnFromCurrentFolder() {
    if (!this.selectedFolderSubject.value) {
      return;
    }

    this.selectedFolderSubject.value.columns--;
    await this.updateFolder(this.selectedFolderSubject.value);
  }
}
