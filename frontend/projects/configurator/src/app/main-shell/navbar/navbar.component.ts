import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NavbarLogoComponent} from './navbar-logo/navbar-logo.component';
import {NavbarSidebarToggleComponent} from './navbar-sidebar-toggle/navbar-sidebar-toggle.component';
import {NgbTooltip} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-navbar',
  imports: [
    NavbarLogoComponent,
    NavbarSidebarToggleComponent,
    NgbTooltip
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  @Output() sidebarToggle = new EventEmitter<void>();
  @Input() sidebarOpen: boolean = true;
}
