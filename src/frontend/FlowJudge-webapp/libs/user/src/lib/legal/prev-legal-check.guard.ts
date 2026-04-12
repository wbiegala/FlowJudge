import { Store } from '@ngxs/store';
import { inject } from '@angular/core';
import { CanMatchFn, Route, UrlSegment } from '@angular/router';
import { Navigate } from '@ngxs/router-plugin';
import { UserLegalService } from './user-legal.service';

export const prevLegalCheckGuard: CanMatchFn  = (route: Route, segments: UrlSegment[]) => {
  const legalService = inject(UserLegalService);
  const store = inject(Store);

  const currentState = legalService.isLegal();

  if (currentState === null || currentState === true) {
    store.dispatch(new Navigate(["/"]));
    return false;
  }

  return true;
}
