import { Route } from '@angular/router';
import { unauthenticatedGuard } from './unauthenticated.guard';

export const authRoutes: Route[] = [
  {
    path: 'confirm-registration',
    loadComponent: () => import('./components/registration-confirmation.component').then(m => m.RegistrationConfirmationComponent)
  },
  {
    path: 'logout',
    canActivate: [ unauthenticatedGuard ],
    loadComponent: () => import('./components/logout.component').then(m => m.LogoutComponent)
  },
  {
    path: 'session-expired',
    canActivate: [ unauthenticatedGuard ],
    loadComponent: () => import('./components/session-expired.component').then(m => m.SessionExpiredComponent)
  },
  {
    path: 'no-access',
    loadComponent: () => import('./components/no-access.component').then(m => m.NoAccessComponent)
  }
];
