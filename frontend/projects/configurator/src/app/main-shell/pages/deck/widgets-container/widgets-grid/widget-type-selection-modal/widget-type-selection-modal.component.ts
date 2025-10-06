import { Component } from '@angular/core';
import {BaseModalComponent} from '../../../../../../common/ui/modals/base-modal/base-modal.component';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {WidgetType} from '../../../../../../common/enums/widget-type.enum';
import {WidgetTypeButtonComponent} from './widget-type-button/widget-type-button.component';

@Component({
  selector: 'app-widget-type-selection-modal',
  imports: [
    BaseModalComponent,
    WidgetTypeButtonComponent
  ],
  templateUrl: './widget-type-selection-modal.component.html'
})
export class WidgetTypeSelectionModalComponent {

  constructor(protected activeModal: NgbActiveModal) {
  }

  public get types(): WidgetType[] {
    return Object.values(WidgetType)
      .filter((value): value is WidgetType => typeof value === 'number');
  }
}
