import { Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { HomePageComponent } from './home-page/home-page.component';
import { authRoutes, tokenExchangeGuard } from '@flow-judge-webapp/auth';

export const appRoutes: Route[] = [
  {
    path: '',
    canActivate: [tokenExchangeGuard],
    component: RoutingLayoutComponent,
    children: [
      {
        path: '',
        component: HomePageComponent
      },
      ...authRoutes
    ]
  }
];
