import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { InitializeEditIntegration } from '../../store/integration-details/integration-details.actions';

export const editIntegrationResolver: ResolveFn<unknown> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const id = route.paramMap.get('id');

  if (!id) {
    throw new Error('Missing integration id');
  }

  return inject(Store).dispatch(new InitializeEditIntegration(id));
}
