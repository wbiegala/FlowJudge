import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { inject } from '@angular/core';
import { UserState } from './store/user.state';
import { Navigate } from '@ngxs/router-plugin';
import { filter, map, take } from 'rxjs';

/**
 * Pass only authenticated user
 */
export const authenticatedGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const store = inject(Store);

  return store.select(UserState.isAuthenticated).pipe(
    filter(isAuth => isAuth !== null),
    take(1),
    map(isAuth => {
      if (isAuth) {
        return true;
      } else {
        store.dispatch(new Navigate(["/no-access"]));
        return false;
      }
    }),
  );
}
