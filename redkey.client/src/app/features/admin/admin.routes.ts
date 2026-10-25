import { Routes } from '@angular/router';
import { AdminLayout } from '../../layouts/admin-layout/admin-layout';

export default [
  {
    path: '',
    component: AdminLayout,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./admin-dashboard/admin-dashboard').then(m => m.AdminDashboard)
      },
      {
        path: 'units',
        loadComponent: () => import('./admin-units/admin-units').then(m => m.AdminUnits)
      },
      {
        path: 'units/new',
        loadComponent: () => import('./unit-form/unit-form').then(m => m.UnitForm)
      },
      {
        path: 'units/:id/edit',
        loadComponent: () => import('./unit-form/unit-form').then(m => m.UnitForm)
      },
      {
        path: 'requests',
        loadComponent: () => import('./admin-requests/admin-requests').then(m => m.AdminRequests)
      },
      {
        path: 'opportunities',
        loadComponent: () => import('./admin-opportunities/admin-opportunities').then(m => m.AdminOpportunities)
      },
      {
        path: 'brokers',
        loadComponent: () => import('./admin-brokers/admin-brokers').then(m => m.AdminBrokers)
      },
      {
        path: 'favorites',
        loadComponent: () => import('./admin-favorites/admin-favorites').then(m => m.AdminFavorites)
      },
      {
        path: 'profile',
        loadComponent: () => import('../account/profile/profile').then(m => m.Profile)
      },
      { path: '**', redirectTo: '' }
    ]
  }
] satisfies Routes;
