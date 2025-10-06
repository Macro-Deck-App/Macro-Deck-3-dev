import {
  ChangeDetectorRef,
  Component,
  DestroyRef,
  HostListener,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  MatTree,
  MatTreeNode,
  MatTreeNodeDef,
  MatTreeNodePadding
} from '@angular/material/tree';
import {CdkContextMenuTrigger, CdkMenu, CdkMenuItem, CdkMenuTrigger} from '@angular/cdk/menu';
import {FolderWithChildsModel} from '../../../../common/models/folder-with-childs.model';
import {FolderService} from '../../../../common/services/folder.service';
import {FolderModel} from '../../../../common/models/folder.model';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CdkTreeNodeToggle} from '@angular/cdk/tree';
import {CreateEditFolderModalComponent} from './create-edit-folder-modal/create-edit-folder-modal.component';
import {ModalService} from '../../../../common/services/modal.service';
import {
  BaseConfirmationModalComponent
} from '../../../../common/ui/modals/base-confirmation-modal/base-confirmation-modal.component';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-folder-navigation',
  imports: [
    MatTree,
    MatTreeNode,
    MatTreeNodeDef,
    CdkMenu,
    CdkMenuItem,
    CdkContextMenuTrigger,
    MatTreeNodePadding,
    CdkTreeNodeToggle,
    CdkMenuTrigger,
    BaseConfirmationModalComponent
  ],
  templateUrl: './folder-navigation.component.html',
  styleUrl: './folder-navigation.component.scss'
})
export class FolderNavigationComponent implements OnInit {
  @ViewChild(MatTree, {static: true}) tree!: MatTree<FolderWithChildsModel>;
  public dataSource: FolderWithChildsModel[] = [];
  public childrenAccessor = (node: FolderWithChildsModel) => node.childs ?? [];
  public hasChild = (node: FolderWithChildsModel) => !!node.childs && node.childs.length > 0;
  public selectedFolder: FolderModel | null = null;

  private destroyRef = inject(DestroyRef);

  constructor(private folderService: FolderService,
              private cdr: ChangeDetectorRef,
              protected modal: ModalService,
              private toasterService: ToastrService) {
  }

  public ngOnInit(): void {
    this.folderService.foldersSubject
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(folders => {
        if (folders) {
          const initialLoad = this.dataSource.length < 1;

          // Collect expanded node IDs recursively
          const expandedIds = new Set<number>();
          const collectExpanded = (nodes: FolderWithChildsModel[] = []) => {
            for (const node of nodes) {
              if (this.tree.isExpanded(node)) {
                expandedIds.add(node.id);
              }
              if (node.childs?.length) {
                collectExpanded(node.childs);
              }
            }
          };
          collectExpanded(this.dataSource); // collect from current data before replacing

          this.dataSource = folders;
          this.cdr.detectChanges();

          queueMicrotask(() => {
            const expandMatching = (nodes: FolderWithChildsModel[] = []) => {
              for (const node of nodes) {
                if (expandedIds.has(node.id)) {
                  this.tree.expand(node);
                }
                if (node.childs?.length) {
                  expandMatching(node.childs);
                }
              }
            };

            if (initialLoad) {
              this.loadExpandedState();
            } else {
              expandMatching(folders);
            }

            if (!this.selectedFolder && this.dataSource.length > 0) {
              this.folderService.selectFolder(folders[0]);
            }
          });
        } else {
          this.dataSource = [];
        }
      });

    this.folderService.selectedFolderSubject
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(folder => {
        this.selectedFolder = folder;
      });
  }

  @HostListener('contextmenu', ['$event'])
  public onRightClick(event: MouseEvent) {
    event.preventDefault();
  }

  public onNodeClick(node: FolderWithChildsModel) {
    this.folderService.selectFolder(node);
  }

  public createFolderInRoot() {
    this.modal.open(CreateEditFolderModalComponent);
  }

  public createFolderInSelected() {
    const modalRef = this.modal.open(CreateEditFolderModalComponent);
    if (!this.selectedFolder) {
      console.error("No folder is selected!");
      return;
    }

    const parentFolder = this.folderService.findFolderById(this.selectedFolder.id);
    if (!parentFolder) {
      console.error("Selected folder was not found!");
      return;
    }

    modalRef.componentInstance.parentFolder = parentFolder;
  }

  public editFolder() {
    const modalRef = this.modal.open(CreateEditFolderModalComponent);
    modalRef.componentInstance.existingFolder = this.selectedFolder;
  }

  public async deleteFolder(folder: FolderModel) {
    await this.folderService.deleteFolder(folder.id);
    this.toasterService.success(`Folder ${folder.displayName} was deleted.`);
    this.folderService.selectFolder(null);
  }

  public expandedChange() {
    const expandedIds = new Set<number>();
    const collectExpanded = (nodes: FolderWithChildsModel[] = []) => {
      for (const node of nodes) {
        if (this.tree.isExpanded(node)) {
          expandedIds.add(node.id);
        }
        if (node.childs?.length) {
          collectExpanded(node.childs);
        }
      }
    };
    collectExpanded(this.dataSource);
    localStorage.setItem(this.getExpandedStateKey(), JSON.stringify(Array.from(expandedIds)));
  }

  private loadExpandedState() {
    const expandedStateJson = localStorage.getItem(this.getExpandedStateKey());
    if (!expandedStateJson) {
      return;
    }

    const expanded = expandedStateJson ? new Set<number>(JSON.parse(expandedStateJson)) : new Set<number>();
    for (const expandedNode of expanded.values()) {
      const node = this.findNodeById(this.dataSource, expandedNode);
      if (node) {
        this.tree.expand(node);
      }
    }
  }

  private findNodeById(nodes: FolderWithChildsModel[], id: number): FolderWithChildsModel | null {
    for (const node of nodes) {
      if (node.id === id) {
        return node;
      }
      if (node.childs) {
        const found = this.findNodeById(node.childs, id);
        if (found) {
          return found;
        }
      }
    }
    return null;
  }

  private getExpandedStateKey() {
    return `expanded_nodes`;
  }
}
