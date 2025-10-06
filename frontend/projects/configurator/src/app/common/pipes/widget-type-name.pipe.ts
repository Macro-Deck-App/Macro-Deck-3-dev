import { Pipe, PipeTransform } from '@angular/core';
import {WidgetType} from '../enums/widget-type.enum';
import {WidgetRegistryService} from '../services/widget-registry.service';

@Pipe({
  name: 'widgetTypeName'
})
export class WidgetTypeNamePipe implements PipeTransform {

  constructor(private widgetRegistry: WidgetRegistryService) {
  }

  transform(value: WidgetType): string {
    const widget = this.widgetRegistry.getRegisteredWidgetByType(value);
    return widget?.name ?? "Unknown";
  }

}
