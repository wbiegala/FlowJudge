import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Store } from '@ngxs/store';
import { WorkspaceContextState } from './store/workspace-context/workspace-context.state';

export const WorkspaceContextHeaderName = 'X-Workspace-Context';

export const workspaceContextInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const workspaceContextId = store.selectSnapshot(WorkspaceContextState.workspaceContextId);

  if (workspaceContextId === undefined || workspaceContextId === null || workspaceContextId === '') {
    return next(req);
  }

  const requestWithWorkspaceContext = req.clone({
    setHeaders: {
      [WorkspaceContextHeaderName]: workspaceContextId,
    },
  });

  return next(requestWithWorkspaceContext);
};
