import { Component } from '@angular/core';
import {
  BaseWidgetConfigurationUiComponent
} from '../../../common/ui/base-widget-configuration-ui/base-widget-configuration-ui.component';

@Component({
  selector: 'app-graph-widget-configuration-ui',
  imports: [],
  templateUrl: './graph-widget-configuration-ui.component.html',
  styleUrl: './graph-widget-configuration-ui.component.scss'
})
export class GraphWidgetConfigurationUiComponent extends BaseWidgetConfigurationUiComponent {
    public override validateAndSave(): boolean {
        return true;
    }
}
