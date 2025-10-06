import {Component, Input} from '@angular/core';
import {WidgetModel} from '../../models/widget.model';

@Component({
  selector: 'app-base-widget-configuration-ui',
  imports: [],
  template: ''
})
export abstract class BaseWidgetConfigurationUiComponent {
  @Input() public widgetModel!: WidgetModel;
  public abstract validateAndSave(): boolean;
  public size: 'sm' | 'lg' | 'xl' | string = 'sm';
}
