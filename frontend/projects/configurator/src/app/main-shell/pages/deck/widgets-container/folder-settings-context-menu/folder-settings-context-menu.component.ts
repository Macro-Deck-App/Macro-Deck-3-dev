import {Component, Input} from '@angular/core';
import {CdkMenu} from '@angular/cdk/menu';
import {FolderModel} from '../../../../../common/models/folder.model';
import {FolderService} from '../../../../../common/services/folder.service';

@Component({
  selector: 'app-folder-settings-context-menu',
  imports: [
    CdkMenu
  ],
  templateUrl: './folder-settings-context-menu.component.html'
})
export class FolderSettingsContextMenuComponent {
  @Input() public currentFolder: FolderModel | null = null;

  constructor(private folderService: FolderService) {
  }

  public get canAddRow() {
    return this.currentFolder?.rows && this.currentFolder.rows < 10;
  }

  public get canRemoveRow() {
    return this.currentFolder?.rows && this.currentFolder.rows > 1;
  }

  public get canAddColumn() {
    return this.currentFolder?.columns && this.currentFolder.columns < 10;
  }

  public get canRemoveColumn() {
    return this.currentFolder?.columns && this.currentFolder.columns > 1;
  }

  async addRow() {
    await this.folderService.addRowToCurrentFolder();
  }

  async removeRow() {
    await this.folderService.removeRowFromCurrentFolder();
  }

  async addColumn() {
    await this.folderService.addColumnToCurrentFolder();
  }

  async removeColumn() {
    await this.folderService.removeColumnFromCurrentFolder();
  }
}
