import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {NgClass} from '@angular/common';

export type ButtonState = 'idle' | 'loading' | 'success' | 'error';
export type ButtonRole = 'default' | 'success' | 'cancel' | 'danger';
export type ButtonSize = 'small' | 'medium' | 'large';

@Component({
  selector: 'app-loading-button',
  imports: [
    NgClass
  ],
  templateUrl: './loading-button.component.html',
  styleUrl: './loading-button.component.scss'
})
export class LoadingButtonComponent implements OnChanges {
  @Input() state: ButtonState = 'idle';
  @Input() disabled = false;
  @Input() type: 'button' | 'submit' = 'button';
  @Input() errorMessage?: string | null = null;
  @Output() clicked = new EventEmitter<void>();
  @Input() role: ButtonRole = 'default';
  @Input() size: ButtonSize = 'medium';

  localErrorMessage = '';

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes['state']?.currentValue === 'error' && this.errorMessage) {
      this.localErrorMessage = this.errorMessage;
    }
  }

  protected handleClick(): void {
    if (this.state === 'loading') {
      return;
    }

    if (this.state === 'error') {
      this.localErrorMessage = '';
    }
    this.clicked.emit();
  }

  protected get buttonClasses(): string {
    if (this.disabled && this.state === 'idle') {
      return 'btn-primary cursor-not-allowed';
    }
    if (this.state === 'success') {
      return 'btn-success';
    }
    if (this.state === 'error') {
      return 'btn-danger';
    }

    switch (this.role) {
      case "default":
        return 'cursor-pointer btn-primary';
      case "success":
        return 'cursor-pointer btn-success';
      case "cancel":
        return 'cursor-pointer btn-secondary';
      case "danger":
        return 'cursor-pointer btn-danger';
    }
  }
}
