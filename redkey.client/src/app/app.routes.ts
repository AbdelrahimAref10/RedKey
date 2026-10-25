import { Routes } from '@angular/router';
import { UserRole } from './core/api/redkey-api.generated';
import { roleGuard } from './core/auth/auth.guards';

/**
 * 4 layouts, each lazy loaded:
 *  - ''         → landing (public website, login, register)
 *  - /customer  → customer website
 *  - /broker    → broker dashboard
 *  - /admin     → super admin dashboard
 */
export const routes: Routes = [
  {
    path: 'customer',
    canMatch: [roleGuard(UserRole.Customer)],
    loadChildren: () => import('./features/customer/customer.routes')
  },
  {
    path: 'broker',
    canMatch: [roleGuard(UserRole.Broker)],
    loadChildren: () => import('./features/broker/broker.routes')
  },
  {
    path: 'admin',
    canMatch: [roleGuard(UserRole.SuperAdmin)],
    loadChildren: () => import('./features/admin/admin.routes')
  },
  {
    path: '',
    loadChildren: () => import('./features/landing/landing.routes')
  }
];
