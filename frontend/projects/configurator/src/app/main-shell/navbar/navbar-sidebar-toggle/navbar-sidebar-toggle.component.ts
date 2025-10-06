import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
  selector: 'app-navbar-sidebar-toggle',
  imports: [],
  templateUrl: './navbar-sidebar-toggle.component.html'
})
export class NavbarSidebarToggleComponent {
  @Output() toggle = new EventEmitter<void>();
  @Input() open: boolean = true;
}
