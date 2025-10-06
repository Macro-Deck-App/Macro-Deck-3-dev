import {Component, Input} from '@angular/core';
import {SidebarButtonComponent} from './sidebar-button/sidebar-button.component';

@Component({
  selector: 'app-sidebar',
  imports: [
    SidebarButtonComponent
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  @Input() open: boolean = true;
}
