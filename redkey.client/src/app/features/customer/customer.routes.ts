import { Routes } from '@angular/router';
import { CustomerLayout } from '../../layouts/customer-layout/customer-layout';
import { siteRoutes } from '../site/site.routes';

export default [
  {
    path: '',
    component: CustomerLayout,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./customer-home/customer-home').then(m => m.CustomerHome)
      },
      ...siteRoutes('/customer'),
      {
        path: 'favorites',
        loadComponent: () => import('./favorites/favorites').then(m => m.Favorites)
      },
      {
        path: 'profile',
        loadComponent: () => import('../account/profile/profile').then(m => m.Profile)
      },
      { path: '**', redirectTo: '' }
    ]
  }
] satisfies Routes;
