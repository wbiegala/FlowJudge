import { Injectable, inject } from '@angular/core';
import { NavigationExtras, Params } from '@angular/router';
import { Store } from '@ngxs/store';
import { Navigate } from '@ngxs/router-plugin';
import { WorkspaceContextState } from './store/workspace-context/workspace-context.state';

@Injectable({
  providedIn: 'root',
})
export class WorkspaceNavigationService {
  private readonly store = inject(Store);

  navigate(
    path: any[],
    queryParams?: Params,
    extras?: NavigationExtras,
  ) {
    const normalizedPath = this.normalizePath(path);

    const isWorkspaceContext = this.store.selectSnapshot(
      WorkspaceContextState.isWorkspaceContext,
    );

    if (!isWorkspaceContext) {
      return this.store.dispatch(
        new Navigate(normalizedPath, queryParams, extras),
      );
    }

    const workspaceContextId = this.store.selectSnapshot(
      WorkspaceContextState.workspaceContextId,
    );

    if (!workspaceContextId) {
      return this.store.dispatch(
        new Navigate(normalizedPath, queryParams, extras),
      );
    }

    return this.store.dispatch(
      new Navigate(
        ['/w', workspaceContextId, ...normalizedPath],
        queryParams,
        extras,
      ),
    );
  }

  private normalizePath(path: any[]): any[] {
    if (path.length === 0) {
      return [];
    }

    const [firstSegment, ...rest] = path;

    if (typeof firstSegment === 'string' && firstSegment.startsWith('/')) {
      return [firstSegment.substring(1), ...rest];
    }

    return path;
  }
}
