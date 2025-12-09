import { Component, Input, ViewChild, ViewContainerRef } from '@angular/core';
import { EdgeInsets } from '../../models';
import { edgeInsetsToStyle } from '../../utils';

@Component({
  selector: 'md-container',
  template: `
    <div 
      [class]="customClasses"
      [style]="customCss"
      [style.background-color]="backgroundColor"
      [style.width.px]="width"
      [style.height.px]="height"
      [style.border-radius]="borderRadiusStyle"
      [style.border]="borderStyle"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle"
      [style.display]="alignment ? 'flex' : undefined"
      [style.justify-content]="justifyContent"
      [style.align-items]="alignItems">
      <ng-container #childContainer></ng-container>
    </div>
  `,
  standalone: true
})
export class MdContainerComponent {
  @Input() backgroundColor?: string;
  @Input() width?: number;
  @Input() height?: number;
  @Input() borderRadius?: any;
  @Input() border?: any;
  @Input() alignment?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  @ViewChild('childContainer', { read: ViewContainerRef, static: true })
  childContainer!: ViewContainerRef;

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }

  get borderRadiusStyle(): string | undefined {
    if (!this.borderRadius) return undefined;
    if (typeof this.borderRadius === 'number') {
      return `${this.borderRadius}px`;
    }
    const { topLeft, topRight, bottomRight, bottomLeft } = this.borderRadius;
    return `${topLeft}px ${topRight}px ${bottomRight}px ${bottomLeft}px`;
  }

  get borderStyle(): string | undefined {
    if (!this.border) return undefined;
    return `${this.border.width}px ${this.border.style || 'solid'} ${this.border.color}`;
  }

  get justifyContent(): string | undefined {
    if (!this.alignment) return undefined;
    const align = this.alignment.toLowerCase();
    if (align.includes('left')) return 'flex-start';
    if (align.includes('right')) return 'flex-end';
    if (align.includes('center')) return 'center';
    return undefined;
  }

  get alignItems(): string | undefined {
    if (!this.alignment) return undefined;
    const align = this.alignment.toLowerCase();
    if (align.startsWith('top')) return 'flex-start';
    if (align.startsWith('bottom')) return 'flex-end';
    if (align.startsWith('center')) return 'center';
    return undefined;
  }
}
