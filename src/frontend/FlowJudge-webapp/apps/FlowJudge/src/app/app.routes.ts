import { Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { authenticatedGuard, authRoutes, tokenExchangeGuard } from '@flow-judge-webapp/auth';
import { userLegalGuard, userRoutes } from '@flow-judge-webapp/user';
import { workspacesRoutes, workspaceContextGuard } from '@flow-judge-webapp/workspaces';

export const appRoutes: Route[] = [
  {
    path: '',
    canActivate: [tokenExchangeGuard],
    canActivateChild: [userLegalGuard],
    component: RoutingLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./home-page/home-page.component').then(m => m.HomePageComponent)
      },
      {
        path: 'workspaces',
        canActivate: [authenticatedGuard],
        canActivateChild: [authenticatedGuard],
        children: workspacesRoutes,
      },
      {
        path: 'w/:workspaceId',
        canActivate: [authenticatedGuard, workspaceContextGuard],
        canActivateChild: [authenticatedGuard],
        children: [
          {
            path: '',
            loadComponent: () => import('./home-page/home-page.component').then(m => m.HomePageComponent),
          },
          {
            path: 'workspaces',
            children: workspacesRoutes,
          },
          {
            path: 'integrations',
            loadChildren: () => import('@flow-judge-webapp/integrations').then(m => m.integrationsRoutes),
          }
        ],
      },
      {
        path: '',
        children: authRoutes,
      },
      {
        path: '',
        children: userRoutes,
      }
    ]
  }
];
