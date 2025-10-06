import { Component } from '@angular/core';
import {DeveloperNavbarComponent} from './developer-navbar/developer-navbar.component';
import {RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-developer',
  imports: [
    DeveloperNavbarComponent,
    RouterOutlet
  ],
  templateUrl: './developer.component.html',
  styleUrl: './developer.component.scss'
})
export class DeveloperComponent {

}
