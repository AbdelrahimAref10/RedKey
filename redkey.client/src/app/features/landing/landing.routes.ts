import { Routes } from '@angular/router';
import { UserRole } from '../../core/api/redkey-api.generated';
import { guestGuard, roleGuard } from '../../core/auth/auth.guards';
import { LandingLayout } from '../../layouts/landing-layout/landing-layout';

/**
 * The public website. Visitors and signed-in customers share the same pages;
 * customer-only pages (profile, favorites) are guarded.
 */
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
      {
        path: 'properties',
        loadComponent: () => import('../site/property-list/property-list').then(m => m.PropertyList)
      },
      {
        path: 'properties/:id',
        loadComponent: () => import('../site/property-details/property-details').then(m => m.PropertyDetails)
      },
      {
        path: 'invest',
        loadComponent: () => import('../site/invest-list/invest-list').then(m => m.InvestList)
      },
      {
        path: 'invest/:id',
        loadComponent: () => import('../site/opportunity-details/opportunity-details').then(m => m.OpportunityDetails)
      },
      {
        path: 'contact',
        loadComponent: () => import('../site/contact-page/contact-page').then(m => m.ContactPage)
      },
      {
        path: 'request-sent',
        loadComponent: () => import('../site/request-sent/request-sent').then(m => m.RequestSent)
      },
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
        path: 'profile',
        canMatch: [roleGuard(UserRole.Customer)],
        loadComponent: () => import('../account/profile/profile').then(m => m.Profile)
      },
      {
        path: 'my-units',
        canMatch: [roleGuard(UserRole.Customer)],
        loadComponent: () => import('../account/my-units/my-units').then(m => m.MyUnits)
      },
      {
        path: 'my-investments',
        canMatch: [roleGuard(UserRole.Customer)],
        loadComponent: () => import('../account/my-investments/my-investments').then(m => m.MyInvestments)
      },
      {
        path: 'favorites',
        canMatch: [roleGuard(UserRole.Customer)],
        loadComponent: () => import('../account/favorites/favorites').then(m => m.Favorites)
      },
      {
        path: '**',
        loadComponent: () => import('../not-found/not-found').then(m => m.NotFound)
      }
    ]
  }
] satisfies Routes;
