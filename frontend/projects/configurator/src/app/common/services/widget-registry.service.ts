import {Injectable, Injector, Type} from '@angular/core';
import {WidgetType} from '../enums/widget-type.enum';
import {BaseWidgetComponent} from '../ui/base-widget/base-widget.component';
import {
  BaseWidgetConfigurationUiComponent
} from '../ui/base-widget-configuration-ui/base-widget-configuration-ui.component';
import {ActionButtonWidgetComponent} from '../../widgets/action-button-widget/action-button-widget.component';
import {GraphWidgetComponent} from '../../widgets/graph-widget/graph-widget.component';
import {ImageWidgetComponent} from '../../widgets/image-widget/image-widget.component';
import {MusicPlayerWidgetComponent} from '../../widgets/music-player-widget/music-player-widget.component';

@Injectable({
  providedIn: 'root'
})
export class WidgetRegistryService {
  private readonly widgets: Type<BaseWidgetComponent>[] = [
    ActionButtonWidgetComponent,
    GraphWidgetComponent,
    ImageWidgetComponent,
    MusicPlayerWidgetComponent
  ];

  constructor(private injector: Injector) {}

  public getRegisteredWidgetByType(widgetType: WidgetType): BaseWidgetComponent | null {
    return this.getRegisteredWidgets().find(x => x.widgetType === widgetType) ?? null;
  }

  public getRegisteredWidgets(): BaseWidgetComponent[] {
    return this.widgets.map(widgetClass => this.injector.get(widgetClass));
  }

  public getWidgetByType(widgetType: WidgetType): Type<BaseWidgetComponent> | null {
    const widget = this.getRegisteredWidgets()
      .find(widget => widget.widgetType == widgetType);
    return widget ? widget.constructor as Type<BaseWidgetComponent> : null;
  }

  public getWidgetEditorByType(widgetType: WidgetType): Type<BaseWidgetConfigurationUiComponent> | null {
    return this.getRegisteredWidgets()
      .find(widget => widget.widgetType == widgetType)?.configurationUi ?? null;
  }
}
