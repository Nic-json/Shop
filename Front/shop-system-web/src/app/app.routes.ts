import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/dashboard/dashboard').then((m) => m.Dashboard),
      },

      {
        path: 'clientes',
        loadComponent: () => import('./pages/clientes/clientes').then((m) => m.Clientes),
      },
    ],
  },
];
