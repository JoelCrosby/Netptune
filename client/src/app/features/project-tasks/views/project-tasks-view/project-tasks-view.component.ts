import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { exportTasks } from '@core/store/tasks/tasks.actions';
import { ProjectTasksHubService } from '@core/store/tasks/tasks.hub.service';
import { selectCurrentWorkspaceIdentifier } from '@core/store/workspaces/workspaces.selectors';
import { HeaderAction } from '@core/types/header-action';
import { TaskDialogComponent } from '@entry/dialogs/task-dialog/task-dialog.component';
import { Store } from '@ngrx/store';
import { from } from 'rxjs';
import { first, switchMap } from 'rxjs/operators';

@Component({
  templateUrl: './project-tasks-view.component.html',
  styleUrls: ['./project-tasks-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectTasksViewComponent implements OnInit, OnDestroy {
  secondaryActions: HeaderAction[] = [
    {
      label: 'Export Tasks',
      click: () => this.onExportTasksClicked(),
      icon: 'get_app',
      iconClass: 'material-icons-round',
    },
  ];

  constructor(
    public dialog: MatDialog,
    private store: Store,
    private hubService: ProjectTasksHubService
  ) {}

  ngOnInit() {
    this.store
      .select(selectCurrentWorkspaceIdentifier)
      .pipe(
        first(),
        switchMap((identifier) =>
          from(this.hubService.connect()).pipe(
            switchMap(() => this.hubService.addToGroup(identifier))
          )
        )
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.hubService.disconnect();
  }

  showAddModal() {
    this.dialog.open(TaskDialogComponent, {
      width: '600px',
    });
  }

  onExportTasksClicked() {
    this.store.dispatch(exportTasks());
  }
}
