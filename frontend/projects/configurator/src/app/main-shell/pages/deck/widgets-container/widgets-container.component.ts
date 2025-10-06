import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import {WidgetsGridComponent} from './widgets-grid/widgets-grid.component';
import {CdkMenu, CdkMenuTrigger} from '@angular/cdk/menu';
import {FolderService} from '../../../../common/services/folder.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FolderModel} from '../../../../common/models/folder.model';
import {
  FolderSettingsContextMenuComponent
} from './folder-settings-context-menu/folder-settings-context-menu.component';

@Component({
  selector: 'app-widgets-container',
  imports: [
    WidgetsGridComponent,
    CdkMenu,
    CdkMenuTrigger,
    FolderSettingsContextMenuComponent
  ],
  templateUrl: './widgets-container.component.html'
})
export class WidgetsContainerComponent implements OnInit {

  public currentFolder: FolderModel | null = null;
  private destroyRef = inject(DestroyRef);

  constructor(private folderService: FolderService) {
  }

  public ngOnInit(): void {
    this.folderService.selectedFolderSubject
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: folder => {
          this.currentFolder = folder;
        }
      });
  }
}
