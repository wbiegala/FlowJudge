import { Injectable } from '@angular/core';
import { AppErrorHandler, ErrorHandlerResult, HttpErrorContext } from '@flow-judge-webapp/common';
import { Observable } from 'rxjs';

@Injectable()
export class TokenReceiveErrorHandler implements AppErrorHandler {
  readonly priority = 1;

  canHandle(context: HttpErrorContext): boolean {
    throw new Error('Method not implemented.');
  }

  handle(context: HttpErrorContext): Observable<ErrorHandlerResult> {
    throw new Error('Method not implemented.');
  }
}
