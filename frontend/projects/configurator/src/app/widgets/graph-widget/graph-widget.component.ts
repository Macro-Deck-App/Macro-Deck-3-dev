import {Component, Injectable, Type} from '@angular/core';
import {BaseWidgetComponent} from '../../common/ui/base-widget/base-widget.component';
import {WidgetType} from '../../common/enums/widget-type.enum';
import {
  GraphWidgetConfigurationUiComponent
} from './graph-widget-configuration-ui/graph-widget-configuration-ui.component';

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-graph-widget',
  imports: [],
  templateUrl: './graph-widget.component.html',
  styleUrl: './graph-widget.component.scss'
})
export class GraphWidgetComponent extends BaseWidgetComponent {
  public override name: string = "Graph";
  public override icon: string = "mdi mdi-chart-line";
  public override widgetType: WidgetType = WidgetType.Graph;
  public override configurationUi = GraphWidgetConfigurationUiComponent;
}
