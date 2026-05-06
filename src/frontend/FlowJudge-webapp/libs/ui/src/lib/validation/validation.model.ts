export interface ValidationErrorMessage {
  key: string;
  params?: Record<string, unknown>;
}

export interface LengthValidationError {
  requiredLength: number;
  actualLength: number;
}

export interface MinValidationError {
  min: number;
  actual: number;
}

export interface MaxValidationError {
  max: number;
  actual: number;
}
