import { Component, Input } from '@angular/core';
import { EdgeInsets } from '../models';
import { edgeInsetsToStyle } from '../utils';

@Component({
  selector: 'md-text',
  template: `
    <span 
      [class]="customClasses"
      [style]="customCss"
      [style.font-size.px]="fontSize"
      [style.font-weight]="fontWeight"
      [style.color]="color"
      [style.text-align]="textAlign"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle"
      [style.display]="'inline-block'">
      {{ text }}
    </span>
  `,
  standalone: true
})
export class MdTextComponent {
  @Input() text = '';
  @Input() fontSize?: number;
  @Input() fontWeight?: string;
  @Input() color?: string;
  @Input() textAlign?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }
}
