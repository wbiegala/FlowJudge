import { inject, Injectable } from '@angular/core';
import { AppErrorHandler, ErrorHandlerResult, HttpErrorContext } from '@flow-judge-webapp/common';
import { NotificationService } from '@flow-judge-webapp/ui';
import { Observable, of } from 'rxjs';

@Injectable()
export class DefaultHttpErrorHandler implements AppErrorHandler {
  #notificationService = inject(NotificationService);
  readonly priority = 0;

  canHandle(context: HttpErrorContext): boolean {
    if (context.request.url.endsWith('refresh-token')) {
      return false;
    }

    return true;
  }

  handle(context: HttpErrorContext): Observable<ErrorHandlerResult> {
    this.#notificationService.showError(context.problem.code ?? 'ERRORS.GENERIC');

    return of({ handled: true });
  }
}
