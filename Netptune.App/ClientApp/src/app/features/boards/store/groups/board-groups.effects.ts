import { selectCurrentBoard } from '../boards/boards.selectors';
import { BoardGroupsService } from './board-groups.service';
import { Injectable } from '@angular/core';
import { AppState } from '@core/core.state';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Action, Store } from '@ngrx/store';
import { of } from 'rxjs';
import {
  catchError,
  map,
  switchMap,
  withLatestFrom,
  tap,
  filter,
} from 'rxjs/operators';
import * as actions from './board-groups.actions';
import { MatSnackBar } from '@angular/material';

@Injectable()
export class BoardGroupsEffects {
  loadBoardGroups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actions.loadBoardGroups),
      withLatestFrom(this.store.select(selectCurrentBoard)),
      filter(([_, board]) => board !== undefined),
      switchMap(([_, board]) =>
        this.boardGroupsService.get(board.id).pipe(
          map(boardGroups => actions.loadBoardGroupsSuccess({ boardGroups })),
          catchError(error => of(actions.loadBoardGroupsFail({ error })))
        )
      )
    )
  );

  createBoardGroup$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actions.createBoardGroup),
      switchMap(action =>
        this.boardGroupsService.post(action.boardGroup).pipe(
          map(boardGroup => actions.createBoardGroupSuccess({ boardGroup })),
          catchError(error => of(actions.createBoardGroupFail({ error })))
        )
      )
    )
  );

  deleteBoardGroups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(actions.deleteBoardGroup),
      switchMap(action =>
        this.boardGroupsService.delete(action.boardGroup).pipe(
          tap(() => this.snackbar.open('Board Group Deleted')),
          map(boardGroup => actions.deleteBoardGroupSuccess({ boardGroup })),
          catchError(error => of(actions.deleteBoardGroupFail({ error })))
        )
      )
    )
  );

  constructor(
    private actions$: Actions<Action>,
    private boardGroupsService: BoardGroupsService,
    private store: Store<AppState>,
    private snackbar: MatSnackBar
  ) {}
}