import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import {
  MAT_SNACK_BAR_DATA,
  MatSnackBarAction,
  MatSnackBarActions,
  MatSnackBarLabel,
  MatSnackBarRef,
} from '@angular/material/snack-bar';
import { TranslatePipe } from '@ngx-translate/core';
import { SnackbarData, SnackbarVariant } from '../snackbar-data.model';

type SnackbarTheme = {
  icon: string;
  hostClass: string;
  ariaLabel: string;
};

@Component({
  selector: 'lib-snackbar-notification',
  imports: [
    MatButtonModule,
    MatIconModule,
    MatSnackBarLabel,
    MatSnackBarActions,
    MatSnackBarAction,
    TranslatePipe,
  ],
  template: `
<div class="snackbar-shell" [class]="theme().hostClass">
    <span class="snackbar-content" matSnackBarLabel>
      <span class="icon-wrapper">
        <mat-icon>{{ theme().icon }}</mat-icon>
      </span>

      <span class="message">
        {{ data.message | translate }}
      </span>
    </span>

    <span class="snackbar-actions" matSnackBarActions>
      <button
        mat-icon-button
        matSnackBarAction
        type="button"
        [attr.aria-label]="theme().ariaLabel"
        (click)="onDismissClick()">
        <mat-icon>close</mat-icon>
      </button>
    </span>
  </div>
  `,
  styles: `
    :host {
      display: block;
      width: 100%;
      max-width: 100%;
      color: #f5f7fb;
    }

    .snackbar-shell {
      width: 100%;
      display: flex;
      align-items: center;
      gap: 12px;
      min-height: 56px;
      padding: 4px 4px 4px 0;
      border-radius: 14px;
      background: linear-gradient(180deg, #2a2d34 0%, #23262d 100%);
      border: 1px solid rgba(255, 255, 255, 0.08);
      box-shadow:
        0 10px 30px rgba(0, 0, 0, 0.28),
        inset 0 1px 0 rgba(255, 255, 255, 0.03);
      overflow: hidden;
    }

    .snackbar-shell::before {
      content: '';
      align-self: stretch;
      width: 4px;
      flex: 0 0 4px;
    }

    .snackbar-content {
      flex: 1;
      display: flex;
      align-items: center;
      gap: 12px;
      min-width: 0;
      padding: 0 0 0 8px;
    }

    .icon-wrapper {
      width: 32px;
      height: 32px;
      flex: 0 0 32px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 999px;
    }

    .icon-wrapper mat-icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .message {
      min-width: 0;
      font-size: 14px;
      line-height: 1.4;
      font-weight: 500;
      letter-spacing: 0.01em;
      word-break: break-word;
    }

    .snackbar-actions {
      display: flex;
      align-items: center;
      margin-right: 2px;
    }

    .snackbar-actions button[mat-icon-button] {
      --mdc-icon-button-state-layer-size: 36px;
      border-radius: 10px;
      transition:
        background-color 160ms ease,
        color 160ms ease,
        transform 160ms ease;
    }

    .snackbar-actions button[mat-icon-button]:active {
      transform: scale(0.96);
    }

    .snackbar-shell.error::before {
      background: linear-gradient(180deg, #ff6b6b 0%, #e53935 100%);
    }

    .snackbar-shell.error .icon-wrapper {
      background: rgba(244, 67, 54, 0.14);
    }

    .snackbar-shell.error .icon-wrapper mat-icon {
      color: #ff6b6b;
    }

    .snackbar-shell.error .message {
      color: #f3f6fb;
    }

    .snackbar-shell.error .snackbar-actions button[mat-icon-button] {
      color: rgba(255, 255, 255, 0.72);
    }

    .snackbar-shell.error .snackbar-actions button[mat-icon-button]:hover {
      color: #ffffff;
      background: rgba(255, 255, 255, 0.08);
    }

    .snackbar-shell.warning::before {
      background: linear-gradient(180deg, #ffcc4d 0%, #ff9800 100%);
    }

    .snackbar-shell.warning .icon-wrapper {
      background: rgba(255, 152, 0, 0.16);
    }

    .snackbar-shell.warning .icon-wrapper mat-icon {
      color: #ffb300;
    }

    .snackbar-shell.warning .message {
      color: #f8f4ea;
    }

    .snackbar-shell.warning .snackbar-actions button[mat-icon-button] {
      color: rgba(255, 248, 225, 0.72);
    }

    .snackbar-shell.warning .snackbar-actions button[mat-icon-button]:hover {
      color: #fff8e1;
      background: rgba(255, 193, 7, 0.10);
    }

    .snackbar-shell.success::before {
      background: linear-gradient(180deg, #5ee28d 0%, #22c55e 100%);
    }

    .snackbar-shell.success .icon-wrapper {
      background: rgba(34, 197, 94, 0.16);
    }

    .snackbar-shell.success .icon-wrapper mat-icon {
      color: #4ade80;
    }

    .snackbar-shell.success .message {
      color: #eefbf3;
    }

    .snackbar-shell.success .snackbar-actions button[mat-icon-button] {
      color: rgba(236, 253, 245, 0.72);
    }

    .snackbar-shell.success .snackbar-actions button[mat-icon-button]:hover {
      color: #f0fdf4;
      background: rgba(34, 197, 94, 0.10);
    }

    .snackbar-shell.info::before {
      background: linear-gradient(180deg, #60a5fa 0%, #3b82f6 100%);
    }

    .snackbar-shell.info .icon-wrapper {
      background: rgba(59, 130, 246, 0.16);
    }

    .snackbar-shell.info .icon-wrapper mat-icon {
      color: #60a5fa;
    }

    .snackbar-shell.info .message {
      color: #eff6ff;
    }

    .snackbar-shell.info .snackbar-actions button[mat-icon-button] {
      color: rgba(239, 246, 255, 0.72);
    }

    .snackbar-shell.info .snackbar-actions button[mat-icon-button]:hover {
      color: #f8fbff;
      background: rgba(59, 130, 246, 0.10);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SnackbarNotificationComponent {
  #snackbarRef = inject(MatSnackBarRef<SnackbarNotificationComponent>);
  data = inject(MAT_SNACK_BAR_DATA) as SnackbarData;

  theme = computed<SnackbarTheme>(() => this.getTheme(this.data.variant));

  onDismissClick(): void {
    this.#snackbarRef.dismiss();
  }

  private getTheme(variant: SnackbarVariant): SnackbarTheme {
    switch (variant) {
      case 'error':
        return {
          icon: 'error',
          hostClass: 'error',
          ariaLabel: 'close error notification',
        };
      case 'warning':
        return {
          icon: 'warning',
          hostClass: 'warning',
          ariaLabel: 'close warning notification',
        };
      case 'success':
        return {
          icon: 'check_circle',
          hostClass: 'success',
          ariaLabel: 'close success notification',
        };
      case 'info':
        return {
          icon: 'info',
          hostClass: 'info',
          ariaLabel: 'close info notification',
        };
    }
  }
}
