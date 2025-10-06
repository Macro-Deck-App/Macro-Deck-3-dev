import { Component } from '@angular/core';
import {
  BaseWidgetConfigurationUiComponent
} from '../../../common/ui/base-widget-configuration-ui/base-widget-configuration-ui.component';

@Component({
  selector: 'app-image-widget-configuration-ui',
  imports: [],
  templateUrl: './image-widget-configuration-ui.component.html',
  styleUrl: './image-widget-configuration-ui.component.scss'
})
export class ImageWidgetConfigurationUiComponent extends BaseWidgetConfigurationUiComponent {
    public override validateAndSave(): boolean {
        return true;
    }
}
