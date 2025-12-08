import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LinkRequestModalComponent } from '../ui/modals/link-request-modal/link-request-modal.component';
import { LinkRequestMessage } from 'md-ui';

@Injectable({
  providedIn: 'root'
})
export class LinkRequestService {
  constructor(private modalService: NgbModal) {}

  /**
   * Shows a link request modal and returns the user's response
   */
  async showLinkRequest(request: LinkRequestMessage): Promise<boolean> {
    try {
      const modalRef = this.modalService.open(LinkRequestModalComponent, {
        centered: true,
        size: 'lg',
        backdrop: 'static'
      });

      modalRef.componentInstance.url = request.url;
      modalRef.componentInstance.requestId = request.requestId;

      const result = await modalRef.result;
      
      if (result?.approved) {
        // Open the link in a new tab
        window.open(request.url, '_blank');
        return true;
      }
      
      return false;
    } catch (error) {
      // Modal was dismissed
      return false;
    }
  }
}
