import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorContext } from './http-error-context';
import { AppErrorHandler, ErrorHandlerResult } from './application-error-handler';
import { APP_ERROR_HANDLERS } from './http-error-injection-token';

@Injectable({ providedIn: 'root' })
export class AppErrorDispatcher {
  private readonly handlers = [...inject(APP_ERROR_HANDLERS, { optional: true }) ?? []];

  dispatch(context: HttpErrorContext): Observable<ErrorHandlerResult> {
    const handler = this.resolveHandler(context);

    if (!handler) {
      return of({ handled: false, rethrow: true });
    }

    return handler.handle(context).pipe(
      map(result => result ?? { handled: true, rethrow: true }),
      catchError(() => of({ handled: false, rethrow: true })),
    );
  }

  private resolveHandler(context: HttpErrorContext): AppErrorHandler | null {
    const matching = this.handlers
      .filter(handler => handler.canHandle(context))
      .sort((a, b) => b.priority - a.priority);
    console.log(matching);

    return matching[0] ?? null;
  }
}
