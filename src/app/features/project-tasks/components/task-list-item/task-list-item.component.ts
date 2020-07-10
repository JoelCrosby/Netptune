import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TaskDetailDialogComponent } from '@app/entry/dialogs/task-detail-dialog/task-detail-dialog.component';
import { AppState } from '@core/core.state';
import { TaskStatus } from '@core/enums/project-task-status';
import { TaskViewModel } from '@core/models/view-models/project-task-dto';
import { TextHelpers } from '@core/util/text-helpers';
import { ConfirmDialogComponent } from '@entry/dialogs/confirm-dialog/confirm-dialog.component';
import { Store } from '@ngrx/store';
import * as TaskActions from '@core/store/tasks/tasks.actions';

@Component({
  selector: '[app-task-list-item]',
  templateUrl: './task-list-item.component.html',
  styleUrls: ['./task-list-item.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskListItemComponent {
  @Input() task: TaskViewModel;

  constructor(private store: Store<AppState>, public dialog: MatDialog) {}

  titleClicked() {
    this.dialog.open(TaskDetailDialogComponent, {
      width: '800px',
      data: this.task,
    });
  }

  deleteClicked() {
    this.dialog
      .open(ConfirmDialogComponent, {
        width: '600px',
        data: {
          title: 'Are you sure you want to delete task?',
          content: `Delete task - ${TextHelpers.truncate(this.task.name)}`,
          confirm: 'Delete',
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.store.dispatch(
            TaskActions.deleteProjectTask({ task: this.task })
          );
        }
      });
  }

  markCompleteClicked() {
    this.store.dispatch(
      TaskActions.editProjectTask({
        task: {
          ...this.task,
          status: TaskStatus.Complete,
        },
      })
    );
  }

  moveToBacklogClicked() {
    this.store.dispatch(
      TaskActions.editProjectTask({
        task: {
          ...this.task,
          status: TaskStatus.InActive,
        },
      })
    );
  }
}
