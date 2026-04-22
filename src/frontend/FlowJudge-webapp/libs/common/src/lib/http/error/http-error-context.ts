import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { AppProblem } from './http-error.model';

export interface HttpErrorContext {
  request: HttpRequest<unknown>;
  response: HttpErrorResponse;
  problem: AppProblem;
}
