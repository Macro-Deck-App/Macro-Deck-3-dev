import { Component, Input } from '@angular/core';
import { EdgeInsets } from '../models';
import { edgeInsetsToStyle } from '../utils';

const SIZE_PX: Record<string, number | undefined> = {
  small: 16,
  medium: undefined,
  large: 48
};

@Component({
  selector: 'md-loading',
  template: `
    <div class="d-flex justify-content-center" [style.margin]="marginStyle" [style.padding]="paddingStyle">
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
  standalone: true
})
export class MdLoadingComponent {
  @Input() size = 'medium';
  @Input() color?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }
  get sizeInPx() { return SIZE_PX[this.size?.toLowerCase()] ?? undefined; }
}
