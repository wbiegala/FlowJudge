import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { inject } from '@angular/core';
import { Navigate } from '@ngxs/router-plugin';
import { filter, map, take } from 'rxjs';
import { UserState } from './store/user.state';

/**
 * Pass only non-authenticated user
 */
export const unauthenticatedGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const store = inject(Store);

return store.select(UserState.isAuthenticated).pipe(
    filter(isAuth => isAuth !== null),
    take(1),
    map(isAuth => {
      if (!isAuth) {
        return true;
      } else {
        store.dispatch(new Navigate(["/"]));
        return false;
      }
    }),
  );
}
