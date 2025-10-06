import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../../models';

@Component({
  selector: 'md-container',
  template: `
    <div 
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.background-color]="backgroundColor"
      [style.width.px]="width"
      [style.height.px]="height"
      [style.border-radius]="borderRadiusStyle"
      [style.border]="borderStyle"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle">
      <ng-content></ng-content>
    </div>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdContainerComponent {
  @Input() backgroundColor?: string;
  @Input() width?: number;
  @Input() height?: number;
  @Input() borderRadius?: any;
  @Input() border?: any;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }

  get borderRadiusStyle(): string | undefined {
    if (!this.borderRadius) return undefined;
    if (typeof this.borderRadius === 'number') {
      return `${this.borderRadius}px`;
    }
    return `${this.borderRadius.topLeft}px ${this.borderRadius.topRight}px ${this.borderRadius.bottomRight}px ${this.borderRadius.bottomLeft}px`;
  }

  get borderStyle(): string | undefined {
    if (!this.border) return undefined;
    return `${this.border.width}px ${this.border.style || 'solid'} ${this.border.color}`;
  }
}
