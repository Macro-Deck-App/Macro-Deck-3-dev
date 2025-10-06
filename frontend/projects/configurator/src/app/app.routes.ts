import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./main-shell/main-shell.component').then(m => m.MainShellComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'deck'
      },
      {
        path: 'deck',
        loadComponent: () => import('./main-shell/pages/deck/deck.component').then(m => m.DeckComponent),
      },
      {
        path: 'automations',
        loadComponent: () => import('./main-shell/pages/automations/automations.component').then(m => m.AutomationsComponent),
      },
      {
        path: 'extensions',
        loadComponent: () => import('./main-shell/pages/extensions/extensions.component').then(m => m.ExtensionsComponent),
      },
      {
        path: 'resources',
        loadComponent: () => import('./main-shell/pages/resources/resources.component').then(m => m.ResourcesComponent),
      },
      {
        path: 'developer',
        loadChildren: () => import('./main-shell/pages/developer/developer.routes').then(m => m.routes),
      },
      {
        path: 'settings',
        loadComponent: () => import('./main-shell/pages/settings/settings.component').then(m => m.SettingsComponent),
      },
      {
        path: 'ui-test',
        loadComponent: () => import('./ui-test/ui-test.component').then(m => m.UiTestComponent),
      },
      {
        path: 'ui/:viewId',
        loadComponent: () => import('./ui-container/ui-container.component').then(m => m.UiContainerComponent),
      },
    ]
  },
  {
    path: 'login',
    loadComponent: () => import('./login/login.component').then(m => m.LoginComponent)
  }
];
