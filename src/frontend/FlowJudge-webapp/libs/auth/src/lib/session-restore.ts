import { Store } from '@ngxs/store';
import { TryRestoreTenantContext } from './store/tenant.actions';
import { APP_INITIALIZER } from '@angular/core';

export function restoreSessionFactory(store: Store) {
  return () => store.dispatch(new TryRestoreTenantContext());
}

export const provideRestoreSessionFactory = {
  provide: APP_INITIALIZER,
  useFactory: restoreSessionFactory,
  deps: [Store],
  multi: true
};
