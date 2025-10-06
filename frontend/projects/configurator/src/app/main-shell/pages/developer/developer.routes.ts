import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./developer.component').then(m => m.DeveloperComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'ui-test'
      },
      {
        path: 'ui-test',
        loadComponent: () => import('./ui-test/ui-test.component').then(m => m.UiTestComponent),
      },
      {
        path: 'ui-test/:viewId',
        loadComponent: () => import('./ui-test/ui-test.component').then(m => m.UiTestComponent),
      },

    ]
  },
];
