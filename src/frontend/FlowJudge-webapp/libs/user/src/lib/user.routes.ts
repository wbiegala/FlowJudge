import { authenticatedGuard } from '@flow-judge-webapp/auth';
import { Route } from '@angular/router';
import { prevLegalCheckGuard } from './legal/prev-legal-check.guard';

export const userRoutes: Route[] = [
  {
    canActivate: [ authenticatedGuard, prevLegalCheckGuard ],
    path: 'legal-check',
    loadComponent: () => import('./components/legal-check/legal-check.component').then(m => m.LegalCheckComponent)
  }
];
