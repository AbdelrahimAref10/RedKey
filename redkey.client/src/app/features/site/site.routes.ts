import { Routes } from '@angular/router';

/**
 * Website pages shared by the landing layout ('') and the customer layout ('/customer').
 * `base` is bound to the components' `base` input so their links stay inside the current layout.
 */
export function siteRoutes(base: '' | '/customer'): Routes {
  const data = { base };
  return [
    {
      path: 'properties',
      data,
      loadComponent: () => import('./property-list/property-list').then(m => m.PropertyList)
    },
    {
      path: 'properties/:id',
      data,
      loadComponent: () => import('./property-details/property-details').then(m => m.PropertyDetails)
    },
    {
      path: 'invest',
      data,
      loadComponent: () => import('./invest-list/invest-list').then(m => m.InvestList)
    },
    {
      path: 'invest/:id',
      data,
      loadComponent: () => import('./opportunity-details/opportunity-details').then(m => m.OpportunityDetails)
    },
    {
      path: 'contact',
      data,
      loadComponent: () => import('./contact-page/contact-page').then(m => m.ContactPage)
    },
    {
      path: 'request-sent',
      data,
      loadComponent: () => import('./request-sent/request-sent').then(m => m.RequestSent)
    }
  ];
}
