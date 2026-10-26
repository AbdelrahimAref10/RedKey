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
        data: { mode: 'admin' },
        loadComponent: () => import('../units/unit-editor/unit-editor').then(m => m.UnitEditor)
      },
      {
        path: 'units/:id',
        data: { mode: 'admin' },
        loadComponent: () => import('../units/unit-editor/unit-editor').then(m => m.UnitEditor)
      },
      {
        path: 'unit-types',
        data: { kind: 'unitTypes' },
        loadComponent: () => import('./lookup-admin/lookup-admin').then(m => m.LookupAdmin)
      },
      {
        path: 'cities',
        loadComponent: () => import('./cities-admin/cities-admin').then(m => m.CitiesAdmin)
      },
      {
        path: 'accounting',
        loadComponent: () => import('./admin-accounting/admin-accounting').then(m => m.AdminAccounting)
      },
      {
        path: 'messages',
        loadComponent: () => import('./admin-messages/admin-messages').then(m => m.AdminMessages)
      },
      {
        path: 'messages/:id',
        loadComponent: () => import('./admin-messages/admin-messages').then(m => m.AdminMessages)
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
