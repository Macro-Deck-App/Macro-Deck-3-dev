import {Component, Injectable} from '@angular/core';
import {BaseWidgetComponent} from '../../common/ui/base-widget/base-widget.component';
import {WidgetType} from '../../common/enums/widget-type.enum';
import {
  ActionButtonWidgetConfigurationUiComponent
} from './action-button-widget-configuration-ui/action-button-widget-configuration-ui.component';

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-action-button-widget',
  imports: [],
  templateUrl: './action-button-widget.component.html',
  styleUrl: './action-button-widget.component.scss'
})
export class ActionButtonWidgetComponent extends BaseWidgetComponent {
    public override name: string = "Action button";
    public override icon: string = "mdi mdi-gesture-tap-button";
    public override widgetType: WidgetType = WidgetType.ActionButton;
    public override configurationUi = ActionButtonWidgetConfigurationUiComponent;
}
