import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { catchError, of, switchMap } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { Store } from '@ngxs/store';
import { Authenticate } from './store/authentication.actions';

export const tokenExchangeGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const auth = inject(AuthenticationService);
  const router = inject(Router);
  const store = inject(Store);

  const stateId = route.queryParamMap.get('stateId');

  if (!stateId) {
    return true;
  }

  return auth.exchangeToken(stateId).pipe(
    switchMap(response => store.dispatch(new Authenticate(response.accessToken, response.identityToken))),
    switchMap(() => {
      const pathWithoutQuery = state.url.split('?')[0] || '/';
      return router.navigateByUrl(pathWithoutQuery, { replaceUrl: true });
    }),
    switchMap(ok => of(!!ok)),
    catchError(() => router.navigateByUrl('/login', { replaceUrl: true }).then(() => false))
  );
};
