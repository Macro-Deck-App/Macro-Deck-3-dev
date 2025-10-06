import {WidgetType} from '../enums/widget-type.enum';

export interface CreateWidgetModel {
  type: WidgetType;
  row: number;
  column: number;
  data: any;
}
