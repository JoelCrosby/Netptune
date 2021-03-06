import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { loadBuildInfo } from '@core/store/meta/meta.actions';
import { selectBuildInfo } from '@core/store/meta/meta.selectors';
import { selectWorkspacesLoading } from '@core/store/workspaces/workspaces.selectors';
import { WorkspaceDialogComponent } from '@entry/dialogs/workspace-dialog/workspace-dialog.component';
import * as WorkspaceActions from '@core/store/workspaces/workspaces.actions';
import { Store } from '@ngrx/store';

@Component({
  templateUrl: './workspaces-view.component.html',
  styleUrls: ['./workspaces-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspacesViewComponent implements OnInit {
  buildInfo$ = this.store.select(selectBuildInfo);
  loading$ = this.store.select(selectWorkspacesLoading);

  constructor(private dialog: MatDialog, private store: Store) {}

  ngOnInit() {
    this.store.dispatch(loadBuildInfo());
    this.store.dispatch(WorkspaceActions.loadWorkspaces());
  }

  openWorkspaceDialog() {
    this.dialog.open(WorkspaceDialogComponent, {
      data: null,
      width: '720px',
    });
  }
}
