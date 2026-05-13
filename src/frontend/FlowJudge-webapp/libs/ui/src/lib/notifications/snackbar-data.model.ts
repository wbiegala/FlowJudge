export type SnackbarVariant = 'error' | 'warning' | 'success' | 'info';

export interface SnackbarData {
  message: string;
  messageArgs?: unknown;
  variant: SnackbarVariant;
}
