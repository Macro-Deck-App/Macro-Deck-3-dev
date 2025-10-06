import {Component, HostListener, OnInit} from '@angular/core';
import {BaseModalComponent} from '../../../../../common/ui/modals/base-modal/base-modal.component';
import {FolderModel} from '../../../../../common/models/folder.model';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {FolderService} from '../../../../../common/services/folder.service';
import {FormControl, ReactiveFormsModule, Validators} from '@angular/forms';
import {ButtonState, LoadingButtonComponent} from '../../../../../common/ui/loading-button/loading-button.component';
import {applyToButton} from '../../../../../common/utils/error-utils';
import {ErrorCodeExceptionModel} from '../../../../../common/models/error-code-exception.model';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-create-edit-folder-modal',
  imports: [
    BaseModalComponent,
    ReactiveFormsModule,
    LoadingButtonComponent
  ],
  templateUrl: './create-edit-folder-modal.component.html'
})
export class CreateEditFolderModalComponent implements OnInit {
  public existingFolder: FolderModel | null = null;
  public parentFolder: FolderModel | null = null;

  public folderName: FormControl = new FormControl(null, [Validators.required]);
  public loading = false;
  public errorMessage: string | null = null;

  constructor(protected activeModal: NgbActiveModal,
              private folderService: FolderService,
              private toasterService: ToastrService) {
  }

  public ngOnInit(): void {
    if (this.existingFolder) {
      this.folderName.setValue(this.existingFolder.displayName);
    }
  }

  public get isInvalid(): boolean {
    return this.folderName.invalid
      || !this.folderName.dirty;
  }

  @HostListener('keydown.enter', ['$event'])
  async handleEnter(_: KeyboardEvent) {
    await this.createFolder();
  }

  public async createFolder() {
    if (this.folderName.invalid) {
      return;
    }

    this.loading = true;
    try {
      if (this.existingFolder) {
        this.existingFolder.displayName = this.folderName.value;
        await this.folderService.updateFolder(this.existingFolder);
        this.toasterService.success(`Folder ${this.folderName.value} was updated.`);
      } else {
        await this.folderService.createFolder(this.folderName.value, this.parentFolder?.id);
        this.toasterService.success(`Folder ${this.folderName.value} was created.`);
      }

      this.activeModal.close();
    } catch (error) {
      if (error instanceof ErrorCodeExceptionModel) {
        this.errorMessage = error.message;
        return;
      }

      this.errorMessage = 'Unknown error';
    } finally {
      this.loading = false;
    }
  }
}
