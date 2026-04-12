import { Store } from '@ngxs/store';
import { TryRestoreAuthenticatedUserContext } from './store/authentication.actions';
import { APP_INITIALIZER } from '@angular/core';

export function restoreSessionFactory(store: Store) {
  return () => store.dispatch(new TryRestoreAuthenticatedUserContext());
}

export const provideRestoreSessionFactory = {
  provide: APP_INITIALIZER,
  useFactory: restoreSessionFactory,
  deps: [Store],
  multi: true
};
