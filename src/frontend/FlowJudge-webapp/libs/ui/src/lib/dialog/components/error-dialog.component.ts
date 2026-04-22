import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { DialogData } from '../dialog-data.model';

@Component({
  selector: 'lib-error-dialog',
  imports: [
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatButton,
    MatIcon,
    TranslatePipe,
  ],
  template: `
    <mat-dialog-content class="dialog">
      <div class="header">
        <div class="header-icon">
          <mat-icon aria-hidden="true">error</mat-icon>
        </div>

        <div class="header-text">
          <h2 class="title">{{ 'UI.DIALOG.ERROR.HEADER' | translate }}: {{ data.header | translate}}</h2>
          <p class="message">
            {{ (data.message) | translate }}
          </p>
        </div>
      </div>

      @if (data.details?.length) {
        <section class="details-section">
          <div class="details-title">
            {{ 'UI.DIALOG.ERROR.DETAILS' | translate }}
          </div>

          <div class="details-panel">
            @for (detail of data.details; track $index) {
              <div class="detail-row">
                <span class="detail-bullet"></span>
                <span class="detail-text">{{ detail | translate}}</span>
              </div>
            }
          </div>
        </section>
      }

      <mat-dialog-actions align="end" class="actions">
        <button mat-flat-button mat-dialog-close cdkFocusInitial>
          OK
        </button>
      </mat-dialog-actions>
    </mat-dialog-content>
  `,
  styles: `
    :host {
      display: block;
    }

    .dialog {
      display: block;
      padding: 0;
      overflow: hidden;
      min-width: 440px;
      max-width: 560px;
    }

    .header {
      display: flex;
      gap: 16px;
      align-items: flex-start;
      padding: 24px 24px 20px;
      background: linear-gradient(to bottom, rgba(244, 67, 54, 0.06), transparent);
      border-bottom: 1px solid rgba(0, 0, 0, 0.06);
    }

    .header-icon {
      width: 44px;
      height: 44px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #fdecec;
      color: #c62828;
      flex-shrink: 0;
    }

    .header-icon mat-icon {
      width: 24px;
      height: 24px;
      font-size: 24px;
    }

    .header-text {
      min-width: 0;
    }

    .title {
      margin: 0;
      font-size: 30px;
      line-height: 1.1;
      font-weight: 700;
      color: #1f2937;
    }

    .message {
      margin: 10px 0 0;
      font-size: 15px;
      line-height: 1.5;
      color: #4b5563;
    }

    .details-section {
      padding: 20px 24px 0;
    }

    .details-title {
      margin-bottom: 10px;
      font-size: 12px;
      font-weight: 700;
      letter-spacing: 0.06em;
      text-transform: uppercase;
      color: #6b7280;
    }

    .details-panel {
      padding: 14px 16px;
      border: 1px solid #f1d4d4;
      background: #fff8f8;
      border-radius: 14px;
    }

    .detail-row {
      display: flex;
      gap: 10px;
      align-items: flex-start;
    }

    .detail-row:not(:last-child) {
      margin-bottom: 12px;
      padding-bottom: 12px;
      border-bottom: 1px solid rgba(198, 40, 40, 0.08);
    }

    .detail-bullet {
      width: 8px;
      height: 8px;
      margin-top: 7px;
      border-radius: 50%;
      background: #d32f2f;
      flex-shrink: 0;
    }

    .detail-text {
      font-size: 14px;
      line-height: 1.55;
      color: #374151;
      word-break: break-word;
    }

    .actions {
      padding: 20px 24px 24px;
      margin: 0;
    }

    .actions button {
      min-width: 90px;
      border-radius: 999px;
      font-weight: 600;
    }

    @media (max-width: 599px) {
      .dialog {
        min-width: auto;
      }

      .header,
      .details-section,
      .actions {
        padding-left: 16px;
        padding-right: 16px;
      }

      .title {
        font-size: 24px;
      }
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorDialogComponent {
  readonly dialogRef = inject(MatDialogRef<ErrorDialogComponent>);
  readonly data = inject<DialogData>(MAT_DIALOG_DATA);
}
