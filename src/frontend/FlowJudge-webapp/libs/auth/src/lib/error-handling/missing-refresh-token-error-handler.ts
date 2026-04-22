import { DialogService } from '@flow-judge-webapp/ui';
import { inject, Injectable } from '@angular/core';
import { AppErrorHandler, ErrorHandlerResult, HttpErrorContext } from '@flow-judge-webapp/common';
import { Observable, of } from 'rxjs';

@Injectable()
export class MissingRefreshTokenErrorHandler implements AppErrorHandler {
  readonly priority = 1;
  #dialogService = inject(DialogService);

  canHandle(context: HttpErrorContext): boolean {
    return context.problem.status === 410
      && context.problem.code === 'auth.missing_refresh_token';
  }

  handle(context: HttpErrorContext): Observable<ErrorHandlerResult> {
    this.#dialogService.showErrorDialog(
      'AUTH.ERRORS.AUTH.MISSING_REFRESH_TOKEN.HEADER',
      'AUTH.ERRORS.AUTH.MISSING_REFRESH_TOKEN.MESSAGE',
      ['AUTH.ERRORS.AUTH.MISSING_REFRESH_TOKEN.DETAILS']
    );

    return of({ handled: true });
  }
}
