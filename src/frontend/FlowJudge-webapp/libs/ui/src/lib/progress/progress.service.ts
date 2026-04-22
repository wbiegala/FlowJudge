import { Injectable, computed, signal } from '@angular/core';
import { Observable, defer, finalize } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProgressService {
  private readonly _activeOperations = signal(0);

  readonly isProgress = computed(() => this._activeOperations() > 0);

  start() {
    this._activeOperations.update(value => value + 1);
  }

  stop() {
    this._activeOperations.update(value => Math.max(0, value - 1));
  }

  runInProgressBar<T>(sourceFactory: () => Observable<T>): Observable<T> {
    return defer(() => {
      this.start();

      return sourceFactory().pipe(
        finalize(() => this.stop()),
      );
    });
  }
}
