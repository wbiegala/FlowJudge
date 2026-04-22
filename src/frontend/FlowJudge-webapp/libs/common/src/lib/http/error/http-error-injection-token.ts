import { InjectionToken, Provider } from '@angular/core';
import { AppErrorHandler } from './application-error-handler';

export const APP_ERROR_HANDLERS = new InjectionToken<readonly AppErrorHandler[]>(
  'APP_ERROR_HANDLERS'
);

export function provideAppErrorHandler(type: new (...args: any[]) => AppErrorHandler): Provider {
  return {
    provide: APP_ERROR_HANDLERS,
    useClass: type,
    multi: true,
  };
}
