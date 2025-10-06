import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ViewMetadata {
  viewId: string;
  namespace: string;
  pluginId?: string;
  transportMode: number;
}

@Component({
  selector: 'app-view-sidebar',
  imports: [CommonModule],
  templateUrl: './view-sidebar.component.html',
  styleUrl: './view-sidebar.component.scss'
})
export class ViewSidebarComponent {
  @Input() views: ViewMetadata[] = [];
  @Input() selectedViewId?: string;
  @Output() viewSelected = new EventEmitter<string>();

  onViewClick(viewId: string): void {
    this.viewSelected.emit(viewId);
  }

  getViewName(viewId: string): string {
    // Extract the name from the view ID (e.g., "server.TestCounterView" -> "TestCounterView")
    const parts = viewId.split('.');
    return parts[parts.length - 1];
  }

  getViewNamespace(viewId: string): string {
    // Extract namespace (e.g., "server.TestCounterView" -> "server")
    const parts = viewId.split('.');
    return parts.slice(0, -1).join('.');
  }
}
