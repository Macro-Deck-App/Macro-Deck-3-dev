import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../models';

@Component({
  selector: 'md-button',
  template: `
    <button 
      [ngClass]="computedClasses"
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle"
      [disabled]="disabled"
      (click)="onButtonClick()">
      {{ text }}
      <ng-content></ng-content>
    </button>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdButtonComponent {
  @Input() text?: string;
  @Input() role?: string = 'Primary'; // ButtonRole enum value from backend
  @Input() backgroundColor?: string; // Custom color overrides role
  @Input() textColor?: string; // Custom color overrides role
  @Input() disabled: boolean = false;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() click = new EventEmitter<void>();

  get computedClasses(): string[] {
    const classes = ['btn'];
    
    // If custom colors are provided, don't use role classes
    if (!this.backgroundColor && !this.textColor) {
      const roleClass = this.getBootstrapRoleClass(this.role);
      if (roleClass) {
        classes.push(roleClass);
      }
    }
    
    return classes;
  }

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }

  private getBootstrapRoleClass(role?: string): string {
    if (!role) return 'btn-primary';
    
    switch (role.toLowerCase()) {
      case 'primary': return 'btn-primary';
      case 'secondary': return 'btn-secondary';
      case 'success': return 'btn-success';
      case 'danger': return 'btn-danger';
      case 'warning': return 'btn-warning';
      case 'info': return 'btn-info';
      case 'light': return 'btn-light';
      case 'dark': return 'btn-dark';
      case 'link': return 'btn-link';
      default: return 'btn-primary';
    }
  }

  onButtonClick(): void {
    if (!this.disabled) {
      this.click.emit();
    }
  }
}
