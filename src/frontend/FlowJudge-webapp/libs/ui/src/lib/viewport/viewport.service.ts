import { Injectable, computed, inject } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ViewportService {
  private breakpointObserver = inject(BreakpointObserver);

  readonly isHandset = toSignal(
    this.breakpointObserver.observe(Breakpoints.Handset).pipe(
      map(result => result.matches)
    ),
    { initialValue: false }
  );

  readonly isXSmall = toSignal(
    this.breakpointObserver.observe(Breakpoints.XSmall).pipe(
      map(result => result.matches)
    ),
    { initialValue: false }
  );

  isNarrow = computed(() => this.isHandset() || this.isXSmall());
}
