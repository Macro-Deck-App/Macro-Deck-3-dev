import { Pipe, PipeTransform } from '@angular/core';
import {WidgetRegistryService} from '../services/widget-registry.service';
import {WidgetType} from '../enums/widget-type.enum';

@Pipe({
  name: 'widgetTypeIcon'
})
export class WidgetTypeIconPipe implements PipeTransform {

  constructor(private widgetRegistry: WidgetRegistryService) {
  }

  transform(value: WidgetType): string {
    const widget = this.widgetRegistry.getRegisteredWidgetByType(value);
    return widget?.icon ?? "mdi mdi-help-rhombus-outline";
  }

}
