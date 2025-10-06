import {Component, Input} from '@angular/core';
import {WidgetType} from '../../../../../../../common/enums/widget-type.enum';
import {WidgetTypeNamePipe} from '../../../../../../../common/pipes/widget-type-name.pipe';
import {WidgetTypeIconPipe} from '../../../../../../../common/pipes/widget-type-icon.pipe';

@Component({
  selector: 'app-widget-type-button',
  imports: [
    WidgetTypeNamePipe,
    WidgetTypeIconPipe
  ],
  templateUrl: './widget-type-button.component.html',
  styleUrl: './widget-type-button.component.scss'
})
export class WidgetTypeButtonComponent {
  @Input() widgetType!: WidgetType;
}
