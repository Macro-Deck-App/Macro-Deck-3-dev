import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MdUiViewComponent } from 'md-ui';
import { Subject, takeUntil } from 'rxjs';

/**
 * Zentraler UI Container der dynamisch Views vom Server lädt
 * 
 * Usage:
 * - Route: /ui/:viewId
 * - Beispiel: /ui/server.TestCounterView
 * - Beispiel: /ui/app.macrodeck.exampleplugin.ConfigView
 */
@Component({
  selector: 'app-ui-container',
  standalone: true,
  imports: [CommonModule, MdUiViewComponent],
  template: `
    <div class="ui-container p-4">
      @if (viewId) {
        <md-ui-view [viewId]="viewId"></md-ui-view>
      } @else {
        <div class="alert alert-warning" role="alert">
          <h4 class="alert-heading">
            <i class="mdi mdi-alert"></i> No View ID
          </h4>
          <p>No view ID provided in the route.</p>
        </div>
      }
    </div>
  `,
  styles: [`
    .ui-container {
      width: 100%;
      height: 100%;
    }
  `]
})
export class UiContainerComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroy$ = new Subject<void>();

  viewId: string | null = null;

  ngOnInit() {
    // Subscribe to route parameter changes
    this.route.paramMap.pipe(
      takeUntil(this.destroy$)
    ).subscribe(params => {
      this.viewId = params.get('viewId');
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
