import { Injectable, inject, signal } from '@angular/core';
import { UserLegalService } from './user-legal.service';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserLegalFacade {
  private readonly userService = inject(UserLegalService);

  readonly missings = this.userService.missings;
  readonly isLegal = this.userService.isLegal;

  private readonly isLoading = signal(false);
  private readonly loadedForUserId = signal<string | null>(null);

  async ensureLoaded(userId: string): Promise<void> {
    if (this.isLoading()) {
      return;
    }

    if (this.loadedForUserId() === userId && this.userService.isLegal() !== null) {
      return;
    }

    this.isLoading.set(true);

    try {
      await firstValueFrom(this.userService.getUserLegalState());
      this.loadedForUserId.set(userId);
    } finally {
      this.isLoading.set(false);
    }
  }

  reset(): void {
    this.loadedForUserId.set(null);
    this.userService.missings.set([]);
  }
}
