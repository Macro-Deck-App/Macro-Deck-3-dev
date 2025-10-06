import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EdgeInsets } from '../../models';

@Component({
  selector: 'md-row',
  template: `
    <div 
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.display]="'flex'"
      [style.flex-direction]="'row'"
      [style.align-items]="getAlignItems()"
      [style.justify-content]="getJustifyContent()"
      [style.gap.px]="spacing"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle">
      <ng-content></ng-content>
    </div>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class MdRowComponent {
  @Input() mainAxisAlignment?: string;
  @Input() crossAxisAlignment?: string;
  @Input() spacing?: number;
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

  getJustifyContent(): string {
    switch (this.mainAxisAlignment?.toLowerCase()) {
      case 'start': return 'flex-start';
      case 'center': return 'center';
      case 'end': return 'flex-end';
      case 'spacebetween': return 'space-between';
      case 'spacearound': return 'space-around';
      case 'spaceevenly': return 'space-evenly';
      default: return 'flex-start';
    }
  }

  getAlignItems(): string {
    switch (this.crossAxisAlignment?.toLowerCase()) {
      case 'start': return 'flex-start';
      case 'center': return 'center';
      case 'end': return 'flex-end';
      case 'stretch': return 'stretch';
      default: return 'center';
    }
  }
}
