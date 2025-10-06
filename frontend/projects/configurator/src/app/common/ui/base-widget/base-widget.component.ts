import {Component, Type} from '@angular/core';
import {WidgetType} from '../../enums/widget-type.enum';
import {
  BaseWidgetConfigurationUiComponent
} from '../base-widget-configuration-ui/base-widget-configuration-ui.component';
import {WidgetModel} from '../../models/widget.model';

@Component({
  selector: 'app-base-widget',
  imports: [],
  template: ''
})
export abstract class BaseWidgetComponent {
  public abstract name: string;
  public abstract icon: string;
  public abstract widgetType: WidgetType;
  public abstract configurationUi: Type<BaseWidgetConfigurationUiComponent> | null;
  public widgetModel!: WidgetModel;
}
