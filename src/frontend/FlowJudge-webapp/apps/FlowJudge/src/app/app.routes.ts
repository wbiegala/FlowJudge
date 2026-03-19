import { Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { HomePageComponent } from './home-page/home-page.component';
import { RegistrationConfirmationComponent } from '@flow-judge-webapp/auth';

export const appRoutes: Route[] = [
  {
    path: '',
    canActivate: [],
    component: RoutingLayoutComponent,
    children: [
      {
        path: '',
        component: HomePageComponent
      },
      {
        path: 'confirm-registration',
        component: RegistrationConfirmationComponent
      }
    ]
  }
];
