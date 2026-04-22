import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AppErrorHandler, ErrorHandlerResult, HttpErrorContext } from '@flow-judge-webapp/common';
import { Observable, of } from 'rxjs';

@Injectable()
export class InsufficientPermissionsErrorHandler implements AppErrorHandler {
  readonly priority = 1;
  #router = inject(Router);

  canHandle(context: HttpErrorContext): boolean {
    return context.problem.status === 403
      && context.problem.code === 'auth.insufficient_permissions';
  }

  handle(context: HttpErrorContext): Observable<ErrorHandlerResult> {
    void this.#router.navigate(['/no-access']);

    return of({ handled: true })
  }
}
