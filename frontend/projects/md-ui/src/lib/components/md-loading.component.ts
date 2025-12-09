import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../models';

@Component({
  selector: 'md-loading',
  template: `
    <div 
      class="d-flex justify-content-center"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle">
      <div 
        class="spinner-border" 
        [class.spinner-border-sm]="size === 'small'"
        [style.width.px]="sizeInPx"
        [style.height.px]="sizeInPx"
        [style.color]="color"
        role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdLoadingComponent {
  @Input() size?: string = 'medium';
  @Input() color?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;

  get sizeInPx(): number | undefined {
    switch (this.size?.toLowerCase()) {
      case 'small': return 16;
      case 'medium': return undefined; // Use default
      case 'large': return 48;
      default: return undefined;
    }
  }

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }
}
