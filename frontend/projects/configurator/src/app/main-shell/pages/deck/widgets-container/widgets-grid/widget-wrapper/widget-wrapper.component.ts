import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CdkDrag} from "@angular/cdk/drag-drop";
import {NgbTooltip} from "@ng-bootstrap/ng-bootstrap";
import {WidgetContainerComponent} from "./widget-container/widget-container.component";
import {NgStyle} from '@angular/common';
import {CdkContextMenuTrigger, CdkMenu, CdkMenuItem} from '@angular/cdk/menu';
import {WidgetModel} from '../../../../../../common/models/widget.model';
import {
  BaseConfirmationModalComponent
} from '../../../../../../common/ui/modals/base-confirmation-modal/base-confirmation-modal.component';
import {ModalService} from '../../../../../../common/services/modal.service';

@Component({
  selector: 'app-widget-wrapper',
  imports: [
    CdkDrag,
    NgbTooltip,
    WidgetContainerComponent,
    NgStyle,
    CdkContextMenuTrigger,
    CdkMenu,
    CdkMenuItem,
    BaseConfirmationModalComponent
  ],
  templateUrl: './widget-wrapper.component.html'
})
export class WidgetWrapperComponent {
  @Input() public widget: WidgetModel | null = null;
  @Input() public widgetStyle: { [klass: string]: any; } | null | undefined;
  @Input() public widgetContentStyle: { [p: string]: any } | null | undefined;
  @Input() public index!: number;

  @Output() public createWidget: EventEmitter<void> = new EventEmitter();
  @Output() public deleteWidget: EventEmitter<void> = new EventEmitter();
  @Output() public editWidget: EventEmitter<void> = new EventEmitter();

  constructor(protected modalService: ModalService) {
  }
}
