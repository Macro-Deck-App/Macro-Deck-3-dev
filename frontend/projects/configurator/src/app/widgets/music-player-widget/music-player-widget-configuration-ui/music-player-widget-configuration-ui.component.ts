import { Component } from '@angular/core';
import {
  BaseWidgetConfigurationUiComponent
} from '../../../common/ui/base-widget-configuration-ui/base-widget-configuration-ui.component';

@Component({
  selector: 'app-music-player-widget-configuration-ui',
  imports: [],
  templateUrl: './music-player-widget-configuration-ui.component.html',
  styleUrl: './music-player-widget-configuration-ui.component.scss'
})
export class MusicPlayerWidgetConfigurationUiComponent extends BaseWidgetConfigurationUiComponent{
    public override validateAndSave(): boolean {
        return true;
    }
}
