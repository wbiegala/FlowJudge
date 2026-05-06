import { Provider } from '@angular/core';
import { VALIDATION_ERROR_MAPPERS } from './validation-mappers.token';
import { builtInValidationErrorMapper } from './validation.mapper';

export function provideValidationErrors(): Provider[] {
  return [
    {
      provide: VALIDATION_ERROR_MAPPERS,
      useValue: builtInValidationErrorMapper,
      multi: true,
    },
  ];
}
