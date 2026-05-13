import { AbstractControl } from '@angular/forms';
import { LengthValidationError, MaxValidationError, MinValidationError, ValidationErrorMessage } from './validation.model';

export interface ValidationErrorMapperContext {
  errorName: string;
  errorValue: unknown;
  control: AbstractControl;
}

export type ValidationErrorMapper = (
  context: ValidationErrorMapperContext,
) => ValidationErrorMessage | null;

export const builtInValidationErrorMapper: ValidationErrorMapper = ({
  errorName,
  errorValue,
}) => {
  const keyPrefix = 'UI.VALIDATION.ERRORS';
  switch (errorName) {
    case 'required':
      return {
        key: `${keyPrefix}.required`,
      };

    case 'requiredTrue':
      return {
        key: `${keyPrefix}.requiredTrue`,
      };

    case 'email':
      return {
        key: `${keyPrefix}.email`,
      };

    case 'maxlength': {
      const error = errorValue as LengthValidationError;

      return {
        key: `${keyPrefix}.maxlength`,
        params: {
          requiredLength: error.requiredLength,
          actualLength: error.actualLength,
        },
      };
    }

    case 'minlength': {
      const error = errorValue as LengthValidationError;

      return {
        key: `${keyPrefix}.minlength`,
        params: {
          requiredLength: error.requiredLength,
          actualLength: error.actualLength,
        },
      };
    }

    case 'min': {
      const error = errorValue as MinValidationError;

      return {
        key: `${keyPrefix}.min`,
        params: {
          min: error.min,
          actual: error.actual,
        },
      };
    }

    case 'max': {
      const error = errorValue as MaxValidationError;

      return {
        key: `${keyPrefix}.max`,
        params: {
          max: error.max,
          actual: error.actual,
        },
      };
    }

    case 'pattern':
      return {
        key: `${keyPrefix}.pattern`,
      };

    default:
      return null;
  }
};
