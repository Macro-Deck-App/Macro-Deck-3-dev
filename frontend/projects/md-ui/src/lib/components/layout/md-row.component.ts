import { Component, Input, ViewChild, ViewContainerRef } from '@angular/core';
import { EdgeInsets } from '../../models';
import { edgeInsetsToStyle, mapMainAxisAlignment, mapCrossAxisAlignment } from '../../utils';

@Component({
  selector: 'md-row',
  template: `
    <div 
      [class]="customClasses"
      [style]="customCss"
      [style.display]="'flex'"
      [style.flex-direction]="'row'"
      [style.align-items]="alignItems"
      [style.justify-content]="justifyContent"
      [style.gap.px]="spacing"
      [style.margin]="marginStyle"
      [style.padding]="paddingStyle">
      <ng-container #childContainer></ng-container>
    </div>
  `,
  standalone: true
})
export class MdRowComponent {
  @Input() mainAxisAlignment?: string;
  @Input() crossAxisAlignment?: string;
  @Input() spacing?: number;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  @ViewChild('childContainer', { read: ViewContainerRef, static: true })
  childContainer!: ViewContainerRef;

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }
  get justifyContent() { return mapMainAxisAlignment(this.mainAxisAlignment); }
  get alignItems() { return mapCrossAxisAlignment(this.crossAxisAlignment, 'center'); }
}
