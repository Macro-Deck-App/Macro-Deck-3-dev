import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EdgeInsets } from '../../models';
import { edgeInsetsToStyle } from '../../utils';

@Component({
  selector: 'md-checkbox',
  template: `
    <div 
      class="form-check"
      [class]="customClasses"
      [style]="customCss"
      [style.margin]="marginStyle" 
      [style.padding]="paddingStyle">
      <input 
        class="form-check-input"
        type="checkbox"
        [(ngModel)]="value"
        [disabled]="disabled"
        (ngModelChange)="valueChange.emit($event)" />
      @if (label) {
        <label class="form-check-label">{{ label }}</label>
      }
    </div>
  `,
  standalone: true,
  imports: [FormsModule]
})
export class MdCheckboxComponent {
  @Input() value = false;
  @Input() label?: string;
  @Input() disabled = false;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() valueChange = new EventEmitter<boolean>();

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }
}
