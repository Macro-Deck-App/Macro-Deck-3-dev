import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EdgeInsets } from '../../models';

@Component({
  selector: 'md-switch',
  template: `
    <div 
      class="form-check form-switch"
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.margin]="marginStyle" 
      [style.padding]="paddingStyle">
      <input 
        class="form-check-input"
        type="checkbox"
        [(ngModel)]="value"
        [disabled]="disabled"
        (ngModelChange)="onValueChange($event)" />
      @if (label) {
        <label class="form-check-label">{{ label }}</label>
      }
    </div>
  `,
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class MdSwitchComponent {
  @Input() value: boolean = false;
  @Input() label?: string;
  @Input() disabled: boolean = false;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() valueChange = new EventEmitter<boolean>();

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }

  onValueChange(newValue: boolean): void {
    this.valueChange.emit(newValue);
  }
}
