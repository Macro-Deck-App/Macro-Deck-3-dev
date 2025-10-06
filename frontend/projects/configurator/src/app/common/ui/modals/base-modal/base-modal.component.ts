import {Component, EventEmitter, Input, Output} from '@angular/core';
import {LoadingSpinnerComponent} from '../../loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-base-modal',
  imports: [
    LoadingSpinnerComponent
  ],
  templateUrl: './base-modal.component.html',
  styleUrl: './base-modal.component.scss'
})
export class BaseModalComponent {
  @Input() title: string = "";
  @Output() crossClicked: EventEmitter<void> = new EventEmitter();
  @Input() loading: boolean = false;
}
