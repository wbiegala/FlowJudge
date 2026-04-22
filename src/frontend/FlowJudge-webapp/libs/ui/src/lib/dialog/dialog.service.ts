import { inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogData } from './dialog-data.model';
import { ErrorDialogComponent } from './components/error-dialog.component';

@Injectable({ providedIn: 'root' })
export class DialogService {
  #dialog = inject(MatDialog);

  showErrorDialog(header: string, message: string, details?: Array<string>) {
    const dialogData = {
      header,
      message,
      details,
      icon: 'error'
    } satisfies DialogData;

    return this.#dialog.open(ErrorDialogComponent, {
      data: dialogData
    })
  }
}
