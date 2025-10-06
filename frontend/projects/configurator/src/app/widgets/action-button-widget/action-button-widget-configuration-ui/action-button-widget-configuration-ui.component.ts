import {Component} from '@angular/core';
import {
  BaseWidgetConfigurationUiComponent
} from '../../../common/ui/base-widget-configuration-ui/base-widget-configuration-ui.component';

@Component({
  selector: 'app-action-button-widget-configuration-ui',
  imports: [],
  templateUrl: './action-button-widget-configuration-ui.component.html',
  styleUrl: './action-button-widget-configuration-ui.component.scss'
})
export class ActionButtonWidgetConfigurationUiComponent extends BaseWidgetConfigurationUiComponent {
  public override size = 'lg';

  public override validateAndSave(): boolean {
    return true;
  }

}
