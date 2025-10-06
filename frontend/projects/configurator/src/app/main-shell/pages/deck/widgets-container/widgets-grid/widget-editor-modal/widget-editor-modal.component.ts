import {AfterViewInit, ChangeDetectorRef, Component, ComponentRef, ViewChild, ViewContainerRef} from '@angular/core';
import {WidgetModel} from '../../../../../../common/models/widget.model';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';
import {WidgetService} from '../../../../../../common/services/widget.service';
import {WidgetType} from '../../../../../../common/enums/widget-type.enum';
import {ToastrService} from 'ngx-toastr';
import {WidgetRegistryService} from '../../../../../../common/services/widget-registry.service';
import {BaseModalComponent} from '../../../../../../common/ui/modals/base-modal/base-modal.component';
import {FormsModule} from '@angular/forms';
import {LoadingButtonComponent} from '../../../../../../common/ui/loading-button/loading-button.component';
import {WidgetTypeNamePipe} from '../../../../../../common/pipes/widget-type-name.pipe';
import {CreateWidgetModel} from '../../../../../../common/models/create-widget.model';

@Component({
  selector: 'app-widget-editor-modal',
  imports: [
    BaseModalComponent,
    FormsModule,
    LoadingButtonComponent,
    WidgetTypeNamePipe
  ],
  templateUrl: './widget-editor-modal.component.html',
  styleUrl: './widget-editor-modal.component.scss'
})
export class WidgetEditorModalComponent implements AfterViewInit {
  @ViewChild("editorRef", {read: ViewContainerRef}) vcr!: ViewContainerRef;
  ref!: ComponentRef<any>

  protected widget: WidgetModel = {
    canDecreaseColSpan: false,
    canDecreaseRowSpan: false,
    canIncreaseColSpan: false,
    canIncreaseRowSpan: false,
    column: 0,
    colSpan: 1,
    data: null,
    id: "",
    row: 0,
    rowSpan: 1,
    type: WidgetType.ActionButton
  };

  create: boolean = true;

  constructor(public activeModal: NgbActiveModal,
              private widgetService: WidgetService,
              private toastService: ToastrService,
              private widgetRegistryService: WidgetRegistryService,
              private cdr: ChangeDetectorRef) {
  }

  async ngAfterViewInit() {
    const widgetEditor = this.widgetRegistryService.getWidgetEditorByType(this.widget.type);
    if (!widgetEditor) {
      this.activeModal.close();
      await this.saveWidget();
      return;
    }

    this.ref = this.vcr.createComponent(widgetEditor);
    this.ref.instance.widgetModel = this.widget;
    this.activeModal.update({size: this.ref.instance.size});
    this.ref.changeDetectorRef.detectChanges();
    this.cdr.detectChanges();
  }

  setRowAndColumn(row: number, column: number) {
    this.widget.row = row;
    this.widget.column = column;
  }

  setWidgetType(widgetType: WidgetType) {
    this.widget.type = widgetType;
  }

  setWidget(widget: WidgetModel) {
    this.widget = widget;
    this.create = false;
  }

  async saveWidget() {
    const valid = this.ref?.instance?.validateAndSave() ?? true;
    if (!valid) {
      return;
    }

    if (!this.create) {
      await this.widgetService.updateWidget(this.widget);
      this.activeModal.close();
      this.toastService.success(`Widget updated`);
      return;
    }

    const createWidget: CreateWidgetModel = {
      row: this.widget.row,
      column: this.widget.column,
      type: this.widget.type,
      data: this.widget.data
    };

    await this.widgetService.createWidget(createWidget);
    this.activeModal.close();
    this.toastService.success(`Widget created`);
  }
}
