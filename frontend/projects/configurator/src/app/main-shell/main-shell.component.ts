import {Component, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {NavbarComponent} from './navbar/navbar.component';
import {SidebarComponent} from './sidebar/sidebar.component';

@Component({
  selector: 'app-main-shell',
  imports: [
    RouterOutlet,
    NavbarComponent,
    SidebarComponent
  ],
  templateUrl: './main-shell.component.html'
})
export class MainShellComponent implements OnInit {
  public _sidebarOpen = true;

  public get sidebarOpen(): boolean {
    return this._sidebarOpen;
  }

  public set sidebarOpen(value: boolean) {
    this._sidebarOpen = value;
    const event = new CustomEvent('sidebar-toggled', { detail: { value: value } });
    window.dispatchEvent(event);
  }

  private readonly storageKey = 'sidebarOpen';

  public ngOnInit(): void {
    const stored = localStorage.getItem(this.storageKey);
    if (stored !== null) {
      this.sidebarOpen = stored === 'true';
    }
  }

  public toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
    localStorage.setItem(this.storageKey, this.sidebarOpen.toString());
  }
}
