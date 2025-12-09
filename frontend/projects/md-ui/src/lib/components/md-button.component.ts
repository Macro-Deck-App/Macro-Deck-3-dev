import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../models';
import { edgeInsetsToStyle } from '../utils';

const ROLE_CLASS_MAP: Record<string, string> = {
  primary: 'btn-primary',
  secondary: 'btn-secondary',
  success: 'btn-success',
  danger: 'btn-danger',
  warning: 'btn-warning',
  info: 'btn-info',
  light: 'btn-light',
  dark: 'btn-dark',
  link: 'btn-link'
};

@Component({
  selector: 'md-button',
  template: `
    <button 
      [ngClass]="buttonClasses"
      [class]="customClasses || ''"
      [style]="customCss"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle"
      [disabled]="disabled"
      (click)="handleClick()">
      {{ text }}
      <ng-content></ng-content>
    </button>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdButtonComponent {
  @Input() text?: string;
  @Input() role = 'primary';
  @Input() backgroundColor?: string;
  @Input() textColor?: string;
  @Input() disabled = false;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() click = new EventEmitter<void>();

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }

  get buttonClasses(): string[] {
    const classes = ['btn'];
    if (!this.backgroundColor && !this.textColor) {
      const roleClass = ROLE_CLASS_MAP[this.role?.toLowerCase()] ?? 'btn-primary';
      classes.push(roleClass);
    }
    return classes;
  }

  handleClick() {
    if (!this.disabled) {
      this.click.emit();
    }
  }
}
