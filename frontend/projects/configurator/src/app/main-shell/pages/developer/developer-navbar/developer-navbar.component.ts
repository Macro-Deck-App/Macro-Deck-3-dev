import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';

@Component({
  selector: 'app-developer-navbar',
  imports: [
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './developer-navbar.component.html',
  styleUrl: './developer-navbar.component.scss'
})
export class DeveloperNavbarComponent {

}
