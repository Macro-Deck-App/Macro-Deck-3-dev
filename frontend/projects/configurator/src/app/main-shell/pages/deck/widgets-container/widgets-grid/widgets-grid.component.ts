import {
  AfterViewInit,
  ApplicationRef,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  OnInit,
  ViewChild
} from '@angular/core';
import {fromEvent, identity} from 'rxjs';
import {CdkDragDrop, CdkDropList} from '@angular/cdk/drag-drop';
import {WidgetModel} from '../../../../../common/models/widget.model';
import {FolderService} from '../../../../../common/services/folder.service';
import {LoadingSpinnerComponent} from '../../../../../common/ui/loading-spinner/loading-spinner.component';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {WidgetTypeSelectionModalComponent} from './widget-type-selection-modal/widget-type-selection-modal.component';
import {ModalService} from '../../../../../common/services/modal.service';
import {WidgetService} from '../../../../../common/services/widget.service';
import {WidgetEditorModalComponent} from './widget-editor-modal/widget-editor-modal.component';
import {WidgetWrapperComponent} from './widget-wrapper/widget-wrapper.component';

@Component({
  selector: 'app-widgets-grid',
  imports: [
    CdkDropList,
    LoadingSpinnerComponent,
    WidgetWrapperComponent
  ],
  templateUrl: './widgets-grid.component.html',
  styleUrl: './widgets-grid.component.scss'
})
export class WidgetsGridComponent implements AfterViewInit, OnInit {
  @ViewChild('widgetsWrapper', {static: false}) wrapperElement!: ElementRef;

  widgets: WidgetModel[] | null = null;

  columns: number = 5;
  rows: number = 3;
  widgetSpacing: number = 5;

  buttonSize: number = 10;
  widgetSpacingPoints: number = 0;

  wrapperWidth: number = 0;
  wrapperHeight: number = 0;
  wrapperPaddingX: number = 0;
  wrapperPaddingY: number = 0;

  private destroyRef = inject(DestroyRef);

  constructor(private modalService: ModalService,
              private applicationRef: ApplicationRef,
              private folderService: FolderService,
              private widgetService: WidgetService
  ) {
  }

  public ngOnInit(): void {
    this.widgetService.widgetsSubject
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((widgets: WidgetModel[]) => {
        this.recalculateWidgetSize();
        this.widgets = widgets;
      });
  }

  async ngAfterViewInit() {
    this.recalculateWidgetSize();

    fromEvent<CustomEvent>(window, 'resize')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((_: CustomEvent) => {
        this.recalculateWidgetSize();
      });

    fromEvent<CustomEvent>(window, 'sidebar-toggled')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((_: CustomEvent) => {
        this.recalculateWidgetSize();
      });
  }

  calculateGrid(): void {
    const folder = this.folderService.selectedFolderSubject.value;
    this.rows = folder?.rows ?? 3;
    this.columns = folder?.columns ?? 5;
    const widgets: any[] = [];
    for (let i = 0; i < this.rows * this.columns; i++) {
      widgets[i] = this.getWidgetFromIndex(i);
    }

    this.widgets = widgets;
  }

  calculateWidgetSize(): void {
    if (this.wrapperElement == null) {
      return;
    }

    const wrapperStyle = window.getComputedStyle(this.wrapperElement.nativeElement, null);
    this.wrapperPaddingX = parseInt(wrapperStyle.getPropertyValue('padding-left')) +
      parseInt(wrapperStyle.getPropertyValue('padding-right'));
    this.wrapperPaddingY = parseInt(wrapperStyle.getPropertyValue('padding-top')) +
      parseInt(wrapperStyle.getPropertyValue('padding-bottom'));
    this.wrapperWidth = (this.wrapperElement?.nativeElement.offsetWidth ?? 0) - this.wrapperPaddingX;
    this.wrapperHeight = (this.wrapperElement?.nativeElement.offsetHeight ?? 0) - this.wrapperPaddingY;
    let widgetSizeX = this.wrapperWidth / this.columns;
    let widgetSizeY = this.wrapperHeight / this.rows;
    this.buttonSize = Math.min(widgetSizeX, widgetSizeY);

    this.widgetSpacingPoints = ((this.widgetSpacing / 100) * this.buttonSize) * 72 / 96;
  }

  getWidgetStyle(index: number) {
    const row = Math.trunc(index / this.columns);
    const column = Math.trunc(index % this.columns);
    const widget = this.widgetService.getWidget(row, column);

    const width = this.buttonSize * (widget?.colSpan ?? 1);
    const height = this.buttonSize * (widget?.rowSpan ?? 1);

    const xOffset = (this.wrapperWidth / 2) - ((this.columns * this.buttonSize) / 2); // Offset to center items horizontally
    const yOffset = (this.wrapperHeight / 2) - ((this.rows * this.buttonSize) / 2); // Offset to center items vertically

    const x = xOffset + (column * this.buttonSize);
    const y = yOffset + (row * this.buttonSize);

    const baseFontSize = Math.min(width, height) / 10;

    return {
      'width': width + 'px',
      'height': height + 'px',
      'position': 'absolute',
      'top': y + "px",
      'left': x + "px",
      'z-index': (!widget) ? -1 : 1,
      'font-size': baseFontSize + 'px'
    }
  }

  getWidgetContentStyle() {
    return {
      'margin': this.widgetSpacingPoints + "pt"
    }
  }

  getWidgetFromIndex(index: number): WidgetModel | null {
    const [row, column] = this.getRowAndColumnFromIndex(index);
    return this.widgetService.getWidget(row, column);
  }

  public getRowAndColumnFromIndex(index: number): [number, number] {
    const row = Math.trunc(index / this.columns);
    const column = Math.trunc(index % this.columns);
    return [row, column];
  }

  public async createWidget(widgetIndex: number) {
    const widgetTypeSelector = this.modalService.open(WidgetTypeSelectionModalComponent);

    const widgetType = await widgetTypeSelector.result;
    if (!widgetType) {
      return;
    }

    const widgetEditor = this.modalService.open(WidgetEditorModalComponent);
    const [row, column] = this.getRowAndColumnFromIndex(widgetIndex);
    widgetEditor.componentInstance.setRowAndColumn(row, column);
    widgetEditor.componentInstance.setWidgetType(widgetType);
  }

  editWidget(widgetIndex: number) {
    const modal = this.modalService.open(WidgetEditorModalComponent);
    const [row, column] = this.getRowAndColumnFromIndex(widgetIndex);
    const widget = this.widgetService.getWidget(row, column);
    if (!widget) {
      return;
    }

    modal.componentInstance.setWidget(widget);
  }

  async deleteWidget(widgetIndex: number) {
    const [row, column] = this.getRowAndColumnFromIndex(widgetIndex);
    const widget = this.widgetService.getWidget(row, column);
    if (!widget) {
      return;
    }

    await this.widgetService.deleteWidget(widget.id);
  }

  async increaseColSpan(widgetIndex: number) {
    const widget = this.getWidgetFromIndex(widgetIndex);
    if (widget === null || !widget.canIncreaseColSpan) {
      return;
    }

    widget.colSpan += 1;
    //await this.widgetService.updateWidget(widget);
  }

  async decreaseColSpan(widgetIndex: number) {
    const widget = this.getWidgetFromIndex(widgetIndex);
    if (widget === null || !widget.canDecreaseColSpan) {
      return;
    }

    widget.colSpan -= 1;
    //await this.widgetService.updateWidget(widget);
  }

  async increaseRowSpan(widgetIndex: number) {
    const widget = this.getWidgetFromIndex(widgetIndex);
    if (widget === null || !widget.canIncreaseRowSpan) {
      return;
    }

    widget.rowSpan += 1;
    //await this.widgetService.updateWidget(widget);
  }

  async decreaseRowSpan(widgetIndex: number) {
    const widget = this.getWidgetFromIndex(widgetIndex);
    if (widget === null || !widget.canDecreaseRowSpan) {
      return;
    }

    widget.rowSpan -= 1;
    //await this.widgetService.updateWidget(widget);
  }

  getWidgetAtPosition(x: number, y: number): [number, number] | null {
    const element = document.elementFromPoint(x, y) as HTMLElement;
    if (!element) {
      return null;
    }
    let widgetElement = element.closest('.widget-content-wrapper');
    if (!widgetElement) {
      return null;
    }

    const dataTarget = widgetElement.getAttribute('data-target');
    if (dataTarget === null) {
      return null;
    }

    return this.getRowAndColumnFromIndex(parseInt(dataTarget));
  }

  getWidgetFromElement(element: ElementRef): WidgetModel | null {
    const dataTarget = element.nativeElement.getAttribute('data-target');
    if (dataTarget === null) {
      return null;
    }

    return this.getWidgetFromIndex(parseInt(dataTarget));
  }

  async drop($event: CdkDragDrop<any, any>) {
    const droppedWidget = this.getWidgetAtPosition($event.dropPoint.x, $event.dropPoint.y);
    if (droppedWidget === null) {
      return;
    }

    const draggedWidget = this.getWidgetFromElement($event.item.element);
    if (draggedWidget === null) {
      return;
    }

    //await this.widgetService.updateWidgetPosition(draggedWidget.id, droppedWidget[0], droppedWidget[1]);
  }

  private recalculateWidgetSize() {
    setTimeout(() => {
      this.calculateGrid();
      this.calculateWidgetSize();
      this.applicationRef.tick();
    }, 1);
  }
}
