import { Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { HomePageComponent } from './home-page/home-page.component';
import { authenticatedGuard, authRoutes, tokenExchangeGuard } from '@flow-judge-webapp/auth';
import { userLegalGuard, userRoutes } from '@flow-judge-webapp/user';
import { workspacesRoutes } from '@flow-judge-webapp/workspaces';

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
      {
        path: 'workspaces',
        canActivate: [authenticatedGuard],
        canActivateChild: [authenticatedGuard],
        children: workspacesRoutes,
      },
      ...authRoutes,
      ...userRoutes
    ]
  }
];
