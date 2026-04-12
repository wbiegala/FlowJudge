import { DestroyRef, Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { filter, pairwise, startWith, switchMap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { from } from 'rxjs';
import { AuthenticationState } from '@flow-judge-webapp/auth';
import { UserLegalFacade } from './user-legal-facade.service';

@Injectable({ providedIn: 'root' })
export class LegalStateWatcherService {
  private readonly store = inject(Store);
  private readonly router = inject(Router);
  private readonly legalFacade = inject(UserLegalFacade);
  private readonly destroyRef = inject(DestroyRef);

  init(): void {
    this.store.select(AuthenticationState.userData).pipe(
      startWith(null),
      pairwise(),
      filter(([previous, current]) => previous?.id !== current?.id && !!current?.id),
      switchMap(([_, current]) => from(this.handleAuthenticatedUser(current!.id))),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  private async handleAuthenticatedUser(userId: string): Promise<void> {
    await this.legalFacade.ensureLoaded(userId);

    const isLegal = this.legalFacade.isLegal();
    const currentUrl = this.router.url;
    const isLegalCheckPage = currentUrl.startsWith('/legal-check');

    if (isLegal === false && !isLegalCheckPage) {
      await this.router.navigateByUrl('/legal-check');
    }

    if (isLegal === true && isLegalCheckPage) {
      await this.router.navigateByUrl('/');
    }
  }
}
