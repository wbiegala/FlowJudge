import { EnvironmentInjector, inject, runInInjectionContext } from '@angular/core';
import { CanActivateChildFn, GuardResult, MaybeAsync, Route } from '@angular/router';
import { RoutingLayoutComponent } from './layout/routing-layout.component';
import { authenticatedGuard, tokenExchangeGuard } from '@flow-judge-webapp/auth';
import { from, isObservable, Observable, switchMap } from 'rxjs';

const userLegalGuard: CanActivateChildFn = (childRoute, state) => {
  const injector = inject(EnvironmentInjector);

  return from(import('@flow-judge-webapp/user')).pipe(
    switchMap(m => runInInjectionContext(
      injector,
      () => toObservable(m.userLegalGuard(childRoute, state))
    ))
  );
};

function toObservable(result: MaybeAsync<GuardResult>): Observable<GuardResult> {
  return isObservable(result) ? result : from(Promise.resolve(result));
}

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
        loadChildren: () => import('@flow-judge-webapp/workspaces').then(m => m.workspacesRoutes),
      },
      {
        path: '',
        loadChildren: () => import('@flow-judge-webapp/auth').then(m => m.authRoutes),
      },
      {
        path: '',
        loadChildren: () => import('@flow-judge-webapp/user').then(m => m.userRoutes),
      }
    ]
  }
];
