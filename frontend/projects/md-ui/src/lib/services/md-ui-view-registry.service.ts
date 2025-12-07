import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { HubConnection } from '@microsoft/signalr';

export interface MdUiViewMetadata {
  viewId: string;
  namespace: string;
  transportMode: string;
  pluginId?: string;
  viewType?: any;
}

/**
 * Service for managing UI view registry
 */
@Injectable()
export class MdUiViewRegistryService {
  private viewsSubject = new BehaviorSubject<MdUiViewMetadata[]>([]);
  public views$ = this.viewsSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Load all registered views from the API
   */
  loadViews(): Observable<MdUiViewMetadata[]> {
    return this.http.get<MdUiViewMetadata[]>('/api/ui/views');
  }

  /**
   * Refresh the views list
   */
  async refreshViews(): Promise<void> {
    this.loadViews().subscribe({
      next: (views: MdUiViewMetadata[]) => {
        this.viewsSubject.next(views);
      },
      error: (err) => {
        console.error('[MdUiViewRegistry] Error loading views:', err);
      }
    });
  }

  /**
   * Subscribe to view registry changes via SignalR
   */
  subscribeToChanges(connection: HubConnection): void {
    connection.on('ViewRegistered', (metadata: MdUiViewMetadata) => {
      const currentViews = this.viewsSubject.value;
      const index = currentViews.findIndex(v => v.viewId === metadata.viewId);
      if (index >= 0) {
        // Update existing
        currentViews[index] = metadata;
        this.viewsSubject.next([...currentViews]);
      } else {
        // Add new
        this.viewsSubject.next([...currentViews, metadata]);
      }
    });

    connection.on('ViewUnregistered', (viewId: string) => {
      const currentViews = this.viewsSubject.value;
      this.viewsSubject.next(currentViews.filter(v => v.viewId !== viewId));
    });
  }

  /**
   * Get current views
   */
  getViews(): MdUiViewMetadata[] {
    return this.viewsSubject.value;
  }

  /**
   * Get a specific view by ID
   */
  getView(viewId: string): MdUiViewMetadata | undefined {
    return this.viewsSubject.value.find(v => v.viewId === viewId);
  }
}
