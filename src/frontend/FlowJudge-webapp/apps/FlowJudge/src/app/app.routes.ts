import { Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { HomePageComponent } from './home-page/home-page.component';
import { authRoutes, tokenExchangeGuard } from '@flow-judge-webapp/auth';
import { userLegalGuard, userRoutes } from '@flow-judge-webapp/user';

export const appRoutes: Route[] = [
  {
    path: '',
    canActivate: [tokenExchangeGuard],
    canActivateChild: [userLegalGuard],
    component: RoutingLayoutComponent,
    children: [
      {
        path: '',
        component: HomePageComponent
      },
      ...authRoutes,
      ...userRoutes
    ]
  }
];
