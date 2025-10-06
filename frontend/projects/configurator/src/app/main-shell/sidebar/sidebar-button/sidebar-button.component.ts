import {Component, Input} from '@angular/core';
import {RouterLink, RouterLinkActive, UrlTree} from '@angular/router';
import {NgbTooltip} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sidebar-button',
  imports: [
    RouterLinkActive,
    RouterLink,
    NgbTooltip
  ],
  templateUrl: './sidebar-button.component.html',
  styleUrl: './sidebar-button.component.scss'
})
export class SidebarButtonComponent {
  @Input() text!: string;
  @Input() iconClasses!: string;
  @Input() routerLink: any[] | string | UrlTree | null | undefined;
  @Input() showText: boolean = true;
}

