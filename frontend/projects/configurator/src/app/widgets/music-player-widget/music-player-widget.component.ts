import {Component, Injectable} from '@angular/core';
import {BaseWidgetComponent} from '../../common/ui/base-widget/base-widget.component';
import {WidgetType} from '../../common/enums/widget-type.enum';
import {
  MusicPlayerWidgetConfigurationUiComponent
} from './music-player-widget-configuration-ui/music-player-widget-configuration-ui.component';

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-music-player-widget',
  imports: [],
  templateUrl: './music-player-widget.component.html',
  styleUrl: './music-player-widget.component.scss'
})
export class MusicPlayerWidgetComponent extends BaseWidgetComponent {
  public override name: string = "Music player";
  public override icon: string = "mdi mdi-music-circle-outline";
  public override widgetType: WidgetType = WidgetType.MusicPlayer;
  public override configurationUi = MusicPlayerWidgetConfigurationUiComponent;
}
