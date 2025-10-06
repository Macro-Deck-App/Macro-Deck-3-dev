import {Component, Injectable, Type} from '@angular/core';
import {BaseWidgetComponent} from '../../common/ui/base-widget/base-widget.component';
import {WidgetType} from '../../common/enums/widget-type.enum';
import {
  ImageWidgetConfigurationUiComponent
} from './image-widget-configuration-ui/image-widget-configuration-ui.component';

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-image-widget',
  imports: [],
  templateUrl: './image-widget.component.html',
  styleUrl: './image-widget.component.scss'
})
export class ImageWidgetComponent extends BaseWidgetComponent {
  public override name: string = "Image";
  public override icon: string = "mdi mdi-panorama-variant-outline";
  public override widgetType: WidgetType = WidgetType.Image;
  public override configurationUi = ImageWidgetConfigurationUiComponent;
}
