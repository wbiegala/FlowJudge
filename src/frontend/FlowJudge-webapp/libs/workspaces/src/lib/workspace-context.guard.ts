import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { catchError, map, of } from 'rxjs';
import { SetWorkspaceContext } from './store/workspace-context/workspace-context.actions';
import { WorkspaceContextState } from './store/workspace-context/workspace-context.state';

const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

export const workspaceContextGuard: CanActivateFn = route => {
  const store = inject(Store);
  const router = inject(Router);
  const workspaceId = route.paramMap.get('workspaceId');

  if (!workspaceId || !guidRegex.test(workspaceId)) {
    return router.createUrlTree(['/no-access']);
  }

  const currentWorkspaceId = store.selectSnapshot(WorkspaceContextState.workspaceContextId);

  if (currentWorkspaceId === workspaceId) {
    return true;
  }

  return store.dispatch(new SetWorkspaceContext(workspaceId, { showSuccessNotification: false })).pipe(
    map(() => {
      const loadedWorkspaceId = store.selectSnapshot(WorkspaceContextState.workspaceContextId);
      return loadedWorkspaceId === workspaceId ? true : router.createUrlTree(['/no-access']);
    }),
    catchError(() => of(router.createUrlTree(['/no-access']))),
  );
};
