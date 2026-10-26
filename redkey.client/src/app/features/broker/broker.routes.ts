import { Routes } from '@angular/router';
import { BrokerLayout } from '../../layouts/broker-layout/broker-layout';

export default [
  {
    path: '',
    component: BrokerLayout,
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./broker-dashboard/broker-dashboard').then(m => m.BrokerDashboard)
      },
      {
        path: 'units',
        loadComponent: () => import('./broker-units/broker-units').then(m => m.BrokerUnits)
      },
      {
        path: 'clients',
        loadComponent: () => import('./broker-clients/broker-clients').then(m => m.BrokerClients)
      },
      {
        path: 'clients/new',
        loadComponent: () => import('./client-form/client-form').then(m => m.ClientForm)
      },
      {
        path: 'listings',
        loadComponent: () => import('./broker-listings/broker-listings').then(m => m.BrokerListings)
      },
      {
        path: 'listings/new',
        data: { mode: 'broker' },
        loadComponent: () => import('../units/unit-editor/unit-editor').then(m => m.UnitEditor)
      },
      {
        path: 'listings/:id',
        data: { mode: 'broker' },
        loadComponent: () => import('../units/unit-editor/unit-editor').then(m => m.UnitEditor)
      },
      {
        path: 'profile',
        loadComponent: () => import('../account/profile/profile').then(m => m.Profile)
      },
      { path: '**', redirectTo: '' }
    ]
  }
] satisfies Routes;
