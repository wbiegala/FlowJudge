import { Route } from '@angular/router';
import { unauthenticatedGuard } from './unauthenticated.guard';
import { LogoutComponent } from './components/logout.component';
import { SessionExpiredComponent } from './components/session-expired.component';
import { NoAccessComponent } from './components/no-access.component';
import { RegistrationConfirmationComponent } from './components/registration-confirmation.component';

export const authRoutes: Route[] = [
  {
    path: 'confirm-registration',
    component: RegistrationConfirmationComponent
  },
  {
    path: 'logout',
    canActivate: [ unauthenticatedGuard ],
    component: LogoutComponent
  },
  {
    path: 'session-expired',
    canActivate: [ unauthenticatedGuard ],
    component: SessionExpiredComponent
  },
  {
    path: 'no-access',
    component: NoAccessComponent
  }
];
