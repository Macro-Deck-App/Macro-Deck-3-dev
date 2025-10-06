import { Component, Input } from '@angular/core';
import { EdgeInsets } from '../models';

@Component({
  selector: 'md-text',
  template: `
    <span 
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.font-size.px]="fontSize"
      [style.font-weight]="fontWeightValue"
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
  @Input() text: string = '';
  @Input() fontSize?: number;
  @Input() fontWeight?: string; // Enum value from backend (normal, bold, lighter, bolder)
  @Input() color?: string;
  @Input() textAlign?: string;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;

  get fontWeightValue(): string | undefined {
    if (!this.fontWeight) return undefined;
    
    // Map enum values to CSS font-weight
    switch (this.fontWeight.toLowerCase()) {
      case 'normal': return 'normal';
      case 'bold': return 'bold';
      case 'lighter': return 'lighter';
      case 'bolder': return 'bolder';
      default: return this.fontWeight; // Fallback to original value
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
