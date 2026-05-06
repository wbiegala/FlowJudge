import { InjectionToken } from '@angular/core';
import { ValidationErrorMapper } from './validation.mapper';

export const VALIDATION_ERROR_MAPPERS =
  new InjectionToken<ValidationErrorMapper[]>('VALIDATION_ERROR_MAPPERS');
