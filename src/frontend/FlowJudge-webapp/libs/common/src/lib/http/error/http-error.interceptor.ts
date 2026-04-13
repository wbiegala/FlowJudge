import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AppErrorDispatcher } from './app-error-dispatcher';
import { HttpErrorContext } from './http-error-context';
import { toAppProblem } from './http-error-normalizer';

export const appErrorInterceptor: HttpInterceptorFn = (
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const dispatcher = inject(AppErrorDispatcher);

  return next(request).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      const context: HttpErrorContext = {
        request,
        response: error,
        problem: toAppProblem(error),
      };

      return dispatcher.dispatch(context).pipe(
        switchMap(result => {
          if (result.handled && !result.rethrow) {
            return throwError(() => new Error('HTTP error was handled and suppressed.'));
          }

          return throwError(() => error);
        }),
      );
    }),
  );
};
