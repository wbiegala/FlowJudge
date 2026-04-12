import { authenticatedGuard } from '@flow-judge-webapp/auth';
import { Route } from '@angular/router';
import { LegalCheckComponent } from './components/legal-check/legal-check.component';
import { prevLegalCheckGuard } from './legal/prev-legal-check.guard';

export const userRoutes: Route[] = [
  {
    canActivate: [ authenticatedGuard, prevLegalCheckGuard ],
    path: 'legal-check',
    component: LegalCheckComponent
  }
];
