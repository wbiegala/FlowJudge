import { inject, Injectable } from '@angular/core';
import { AbstractControl, FormArray, FormGroup } from '@angular/forms';
import { VALIDATION_ERROR_MAPPERS } from './validation-mappers.token';
import { ValidationErrorMessage } from './validation.model';

@Injectable({
  providedIn: 'root',
})
export class ValidationErrorService {
  private readonly mappers = inject(VALIDATION_ERROR_MAPPERS, {
    optional: true,
  }) ?? [];

  getFirstError(control: AbstractControl | null): ValidationErrorMessage | null {
    if (!control?.errors) {
      return null;
    }

    const [errorName, errorValue] = Object.entries(control.errors)[0];

    for (const mapper of this.mappers) {
      const result = mapper({
        errorName,
        errorValue,
        control,
      });

      if (result) {
        return result;
      }
    }

    return this.getFallbackError(errorName, errorValue);
  }

  shouldShowError(control: AbstractControl | null): boolean {
    return !!control && control.invalid === true && (control.touched === true || control.dirty === true);
  }

  touchAndValidate(control: AbstractControl): void {
    control.markAsTouched();

    if (control instanceof FormGroup) {
      Object.values(control.controls).forEach(child => {
        this.touchAndValidate(child);
      });
    }

    if (control instanceof FormArray) {
      control.controls.forEach(child => {
        this.touchAndValidate(child);
      });
    }

    control.updateValueAndValidity();
  }

  private getFallbackError(
    errorName: string,
    errorValue: unknown,
  ): ValidationErrorMessage {
    return {
      key: `VALIDATION.ERRORS.${errorName}`,
      params: this.asParams(errorValue),
    };
  }

  private asParams(errorValue: unknown): Record<string, unknown> {
    if (
      typeof errorValue === 'object' &&
      errorValue !== null &&
      !Array.isArray(errorValue)
    ) {
      return errorValue as Record<string, unknown>;
    }

    return {};
  }
}
