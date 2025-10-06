import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdUiViewComponent } from 'md-ui';

@Component({
  selector: 'app-view-content',
  imports: [CommonModule, MdUiViewComponent],
  templateUrl: './view-content.component.html',
  styleUrl: './view-content.component.scss'
})
export class ViewContentComponent {
  @Input() selectedViewId?: string;
}
