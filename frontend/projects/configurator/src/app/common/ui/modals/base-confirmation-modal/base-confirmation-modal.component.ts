import {BaseModalComponent} from '../base-modal/base-modal.component';
import {Component, EventEmitter, HostListener, Input, Output} from '@angular/core';
import {ButtonRole, LoadingButtonComponent} from "../../loading-button/loading-button.component";

@Component({
  selector: 'app-base-confirmation-modal',
  imports: [
    BaseModalComponent,
    LoadingButtonComponent
  ],
  templateUrl: './base-confirmation-modal.component.html'
})
export class BaseConfirmationModalComponent {
  @Output() public confirm = new EventEmitter<void>();
  @Input() public title: string = "Are you sure?";
  @Input() public confirmButtonText: string = "Yes";
  @Input() public cancelButtonText: string = "Cancel";
  @Input() public confirmValue: any;
  @Input() public dismissValue: any;
  @Input() public confirmButtonRole: ButtonRole = "default";
  @Input() public cancelButtonRole: ButtonRole = "cancel";

  @HostListener('keydown.enter', ['$event'])
  public handleEnter(_: KeyboardEvent) {
    this.confirmButtonClicked();
  }

  public confirmButtonClicked() {
    this.confirm.emit(); this.confirmValue ? this.confirmValue() : this.dismissValue()
  }
}
