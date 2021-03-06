import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import {
  ConfirmDialogComponent,
  ConfirmDialogOptions,
} from '@entry/dialogs/confirm-dialog/confirm-dialog.component';

const DEFAULT_CONFIG: ConfirmDialogOptions = {
  acceptLabel: 'Accept',
  cancelLabel: 'Cancel',
  title: 'Confirm ?',
  color: 'primary',
};

@Injectable({ providedIn: 'root' })
export class ConfirmationService {
  constructor(private dialog: MatDialog) {}

  open(
    config: ConfirmDialogOptions = DEFAULT_CONFIG,
    silent: boolean = false
  ): Observable<boolean> {
    if (silent) {
      return of(true);
    }

    const dialogRef = this.dialog.open<
      ConfirmDialogComponent,
      ConfirmDialogOptions,
      boolean
    >(ConfirmDialogComponent, {
      width: '600px',
      data: config,
    });

    return dialogRef.afterClosed();
  }
}
