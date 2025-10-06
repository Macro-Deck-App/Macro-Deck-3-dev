import { Injectable } from '@angular/core';
import {NgbModal, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';

@Injectable({
  providedIn: 'root'
})
export class ModalService {

  constructor(private modal: NgbModal) { }

  public open(content: any, size: 'sm' | 'lg' | 'xl' | string = 'sm'): NgbModalRef {
    return this.modal.open(content, {
      centered: true,
      size: size,
    });
  }

  public get activeInstances(): import("@angular/core").EventEmitter<NgbModalRef[]> {
    return this.modal.activeInstances;
  }

  public dismissAll(reason?: any): void {
    this.modal.dismissAll();
  }

  public hasOpenModals(): boolean {
    return this.modal.hasOpenModals();
  }
}
