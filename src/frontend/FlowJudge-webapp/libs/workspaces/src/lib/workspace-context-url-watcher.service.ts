import { DestroyRef, Injectable, inject } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Store } from '@ngxs/store';
import { filter, map, startWith } from 'rxjs';
import { CleanWorkspaceContext } from './store/workspace-context/workspace-context.actions';
import { WorkspaceContextState } from './store/workspace-context/workspace-context.state';

const workspaceUrlRegex = /^\/w\/[^/]+(?:\/|$)/;

@Injectable({ providedIn: 'root' })
export class WorkspaceContextUrlWatcherService {
  private readonly router = inject(Router);
  private readonly store = inject(Store);
  private readonly destroyRef = inject(DestroyRef);

  init(): void {
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map(event => event.urlAfterRedirects),
      startWith(this.router.url),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(url => this.cleanWorkspaceContextOutsideWorkspaceUrl(url));
  }

  private cleanWorkspaceContextOutsideWorkspaceUrl(url: string): void {
    if (workspaceUrlRegex.test(url)) {
      return;
    }

    const isWorkspaceContext = this.store.selectSnapshot(WorkspaceContextState.isWorkspaceContext);

    if (isWorkspaceContext) {
      this.store.dispatch(new CleanWorkspaceContext());
    }
  }
}
