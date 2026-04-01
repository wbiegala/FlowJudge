import { Store } from '@ngxs/store';
import { TryRestoreUserContext } from './store/user.actions';
import { APP_INITIALIZER } from '@angular/core';

export function restoreSessionFactory(store: Store) {
  return () => store.dispatch(new TryRestoreUserContext());
}

export const provideRestoreSessionFactory = {
  provide: APP_INITIALIZER,
  useFactory: restoreSessionFactory,
  deps: [Store],
  multi: true
};
