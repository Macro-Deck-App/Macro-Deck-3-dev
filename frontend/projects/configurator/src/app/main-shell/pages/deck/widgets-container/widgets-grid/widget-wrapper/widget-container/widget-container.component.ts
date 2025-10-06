import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  Input,
  OnDestroy,
  ViewChild,
  ViewContainerRef
} from '@angular/core';
import {WidgetModel} from '../../../../../../../common/models/widget.model';
import {WidgetRegistryService} from '../../../../../../../common/services/widget-registry.service';

@Component({
  selector: 'app-widget-container',
  imports: [],
  templateUrl: './widget-container.component.html'
})
export class WidgetContainerComponent implements AfterViewInit, OnDestroy {
  @ViewChild("contentRef", {read: ViewContainerRef}) vcr!: ViewContainerRef;
  @Input() widgetData!: WidgetModel | null;

  private ref!: ComponentRef<any>;

  constructor(private cdr: ChangeDetectorRef,
              private widgetRegistry: WidgetRegistryService) {
  }

  public ngAfterViewInit(): void {
    if (!this.widgetData) {
      return;
    }

    const widget = this.widgetRegistry.getWidgetByType(this.widgetData.type);
    if (!widget) {
      return;
    }

    this.ref = this.vcr.createComponent(widget);
    this.ref.instance.widgetModel = this.widgetData;
    this.ref.location.nativeElement.style.width = '100%';
    this.ref.location.nativeElement.style.height = '100%';
    this.ref.changeDetectorRef.detectChanges();
    this.cdr.detectChanges();
  }

  public ngOnDestroy(): void {
    this.vcr.clear();
  }
}
