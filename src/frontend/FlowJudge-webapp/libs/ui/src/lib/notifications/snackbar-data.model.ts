export type SnackbarVariant = 'error' | 'warning' | 'success' | 'info';

export interface SnackbarData {
  message: string;
  variant: SnackbarVariant;
}
