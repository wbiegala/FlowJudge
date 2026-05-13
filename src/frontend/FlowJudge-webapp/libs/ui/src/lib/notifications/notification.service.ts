import { inject, Injectable } from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';
import { SnackbarVariant } from './snackbar-data.model';
import { SnackbarNotificationComponent } from './components/snackbar-notification.component';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  #snackBar = inject(MatSnackBar);

  showWarning(messageTranslationKey: string, messageArgs?: unknown) {
    this.#openSnackbar(messageTranslationKey, 'warning', messageArgs);
  }

  showError(messageTranslationKey: string, messageArgs?: unknown) {
    this.#openSnackbar(messageTranslationKey, 'error', messageArgs);
  }

  showInfo(messageTranslationKey: string, messageArgs?: unknown) {
    this.#openSnackbar(messageTranslationKey, 'info', messageArgs);
  }

  showSuccess(messageTranslationKey: string, messageArgs?: unknown) {
    this.#openSnackbar(messageTranslationKey, 'success', messageArgs);
  }

  #openSnackbar(message: string, variant: SnackbarVariant, messageArgs?: unknown, duration = 5000) {
    this.#snackBar.openFromComponent(SnackbarNotificationComponent, {
      data: { message, messageArgs, variant },
      duration,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['app-snackbar-panel'],
    });
  }
}
