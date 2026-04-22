import { Observable } from 'rxjs';
import { HttpErrorContext } from './http-error-context';

export interface ErrorHandlerResult {
  handled: boolean;
  rethrow?: boolean;
}

export interface AppErrorHandler {
  readonly priority: number;

  canHandle(context: HttpErrorContext): boolean;

  handle(context: HttpErrorContext): Observable<ErrorHandlerResult>;
}
