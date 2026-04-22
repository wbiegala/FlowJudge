import { HttpErrorResponse } from '@angular/common/http';
import { AppProblem, ProblemDetails } from './http-error.model';

export function toAppProblem(error: HttpErrorResponse): AppProblem {
  const payload = isProblemDetails(error.error) ? error.error : null;

  if (!payload) {
    return {
      status: error.status || 0,
      code: null,
      type: null,
      title: null,
      detail: null,
      extensions: {},
      raw: null,
    };
  }

  const { type, title, detail, status, instance, ...rest } = payload;

  return {
    status: status ?? error.status ?? 0,
    code: readProblemCode(payload),
    type: typeof type === 'string' ? type : null,
    title: typeof title === 'string' ? title : null,
    detail: typeof detail === 'string' ? detail : null,
    extensions: rest,
    raw: payload,
  };
}

function isProblemDetails(value: unknown): value is ProblemDetails {
  return typeof value === 'object' && value !== null;
}

function readProblemCode(problem: ProblemDetails): string | null {
  const extensionCode = (problem['code'] ?? problem['errorCode']) as unknown;
  if (typeof extensionCode === 'string' && extensionCode.length > 0) {
    return extensionCode;
  }

  return typeof problem.title === 'string' && problem.title.length > 0
    ? problem.title
    : null;
}
