import { Routes } from '@angular/router';
import { guestGuard } from '../../core/auth/auth.guards';
import { LandingLayout } from '../../layouts/landing-layout/landing-layout';
import { siteRoutes } from '../site/site.routes';

export default [
  {
    path: '',
    component: LandingLayout,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./home/landing-home').then(m => m.LandingHome)
      },
      ...siteRoutes(''),
      {
        path: 'login',
        canMatch: [guestGuard],
        loadComponent: () => import('../auth/login/login').then(m => m.Login)
      },
      {
        path: 'register',
        canMatch: [guestGuard],
        loadComponent: () => import('../auth/register/register').then(m => m.Register)
      },
      {
        path: '**',
        loadComponent: () => import('../not-found/not-found').then(m => m.NotFound)
      }
    ]
  }
] satisfies Routes;
