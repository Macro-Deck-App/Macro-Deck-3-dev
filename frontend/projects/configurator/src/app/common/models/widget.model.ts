import {WidgetType} from '../enums/widget-type.enum';

export interface WidgetModel {
  id: string,
  row: number,
  column: number,
  rowSpan: number,
  canIncreaseRowSpan: boolean,
  canDecreaseRowSpan: boolean,
  colSpan: number,
  canIncreaseColSpan: boolean,
  canDecreaseColSpan: boolean,
  type: WidgetType,
  data: string | null
}
