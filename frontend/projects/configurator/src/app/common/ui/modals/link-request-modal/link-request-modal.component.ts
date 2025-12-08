import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BaseModalComponent } from '../base-modal/base-modal.component';
import { LoadingButtonComponent } from '../../loading-button/loading-button.component';

const TIMEOUT_MS = 300000; // 5 minutes

@Component({
  selector: 'app-link-request-modal',
  imports: [
    BaseModalComponent,
    LoadingButtonComponent
  ],
  templateUrl: './link-request-modal.component.html',
  styleUrl: './link-request-modal.component.scss'
})
export class LinkRequestModalComponent implements OnInit, OnDestroy {
  @Input() url: string = '';
  @Input() requestId: string = '';

  private timeoutId: ReturnType<typeof setTimeout> | null = null;

  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit() {
    // Auto-close modal after timeout
    this.timeoutId = setTimeout(() => {
      this.activeModal.dismiss('timeout');
    }, TIMEOUT_MS);
  }

  ngOnDestroy() {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  approve() {
    this.activeModal.close({ approved: true });
  }

  deny() {
    this.activeModal.close({ approved: false });
  }
}
