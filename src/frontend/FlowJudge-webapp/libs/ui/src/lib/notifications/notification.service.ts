import { inject, Injectable } from '@angular/core';
import {MatSnackBar} from '@angular/material/snack-bar';
import { SnackbarVariant } from './snackbar-data.model';
import { SnackbarNotificationComponent } from './components/snackbar-notification.component';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  #snackBar = inject(MatSnackBar);

  showWarning(messageTranslationKey: string) {
    this.#openSnackbar(messageTranslationKey, 'warning');
  }

  showError(messageTranslationKey: string) {
    this.#openSnackbar(messageTranslationKey, 'error');
  }

  showInfo(messageTranslationKey: string) {
    this.#openSnackbar(messageTranslationKey, 'info');
  }

  showSuccess(messageTranslationKey: string) {
    this.#openSnackbar(messageTranslationKey, 'success');
  }

  #openSnackbar(message: string, variant: SnackbarVariant, duration = 5000) {
    this.#snackBar.openFromComponent(SnackbarNotificationComponent, {
      data: { message, variant },
      duration,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['app-snackbar-panel'],
    });
  }
}
