import { inject } from '@angular/core';
import { CanActivateChildFn, Router, UrlTree } from '@angular/router';
import { Store } from '@ngxs/store';
import { AuthenticationState } from '@flow-judge-webapp/auth';
import { UserLegalFacade } from './user-legal-facade.service';
import { filter, firstValueFrom, take } from 'rxjs';

export const userLegalGuard: CanActivateChildFn = async (route, state): Promise<boolean | UrlTree> => {
  const store = inject(Store);
  const router = inject(Router);
  const legalFacade = inject(UserLegalFacade);

  const isLegalCheckPage = state.url.startsWith('/legal-check');

  const resolvedAuth = await firstValueFrom(
    store.select(AuthenticationState.isAuthenticated).pipe(
      filter((value): value is boolean => value !== null),
      take(1),
    ),
  );

  if (resolvedAuth === false) {
    return true;
  }

  const user = store.selectSnapshot(AuthenticationState.userData);

  if (!user) {
    return true;
  }

  await legalFacade.ensureLoaded(user.id);

  const isLegal = legalFacade.isLegal();

  if (isLegal === false && !isLegalCheckPage) {
    return router.createUrlTree(['/legal-check']);
  }

  if (isLegal === true && isLegalCheckPage) {
    return router.createUrlTree(['/']);
  }

  return true;
};
