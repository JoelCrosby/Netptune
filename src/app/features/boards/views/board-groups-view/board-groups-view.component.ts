import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import * as BoardActions from '@boards/store//boards/boards.actions';
import * as GroupActions from '@boards/store/groups/board-groups.actions';
import * as TagsActions from '@core/store/tags/tags.actions';
import * as GroupSelectors from '@boards/store/groups/board-groups.selectors';
import { Board } from '@core/models/board';
import { BoardGroup } from '@core/models/board-group';
import * as TaskActions from '@core/store/tasks/tasks.actions';
import { ProjectTasksHubService } from '@core/store/tasks/tasks.hub.service';
import { HeaderAction } from '@core/types/header-action';
import { getNewSortOrder } from '@core/util/sort-order-helper';
import { select, Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { filter, first, map, startWith, tap } from 'rxjs/operators';

@Component({
  templateUrl: './board-groups-view.component.html',
  styleUrls: ['./board-groups-view.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [],
})
export class BoardGroupsViewComponent
  implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('importTasksInput') importTasksInput: ElementRef;

  groups$: Observable<BoardGroup[]>;
  selectedBoard$: Observable<Board>;
  selectedBoardName$: Observable<string>;
  loading$: Observable<boolean>;

  private board: Board;

  secondaryActions: HeaderAction[] = [
    {
      label: 'Import Tasks',
      click: () => this.onImportTasksClicked(),
      icon: 'publish',
      iconClass: 'material-icons-outlined',
    },
    {
      label: 'Delete Board',
      click: () => this.onDeleteBoardClicked(),
      icon: 'delete',
      iconClass: 'material-icons-outlined',
    },
  ];

  constructor(
    private store: Store,
    private hubService: ProjectTasksHubService
  ) {}

  ngOnInit() {
    this.groups$ = this.store.select(GroupSelectors.selectAllBoardGroups);
    this.loading$ = this.store
      .select(GroupSelectors.selectBoardGroupsLoading)
      .pipe(startWith(true));

    this.selectedBoardName$ = this.store.pipe(
      select(GroupSelectors.selectBoard),
      filter((board) => !!board),
      tap((board) => {
        this.board = board;
      }),
      map((board) => board && board.name)
    );

    this.store
      .select(GroupSelectors.selectBoardIdentifier)
      .pipe(
        filter((val) => !!val),
        first(),
        tap((identifier) => {
          this.hubService
            .connect()
            .then(() => this.hubService.addToGroup(identifier));
        })
      )
      .subscribe();
  }

  ngAfterViewInit() {
    this.store.dispatch(GroupActions.loadBoardGroups());
    this.store.dispatch(TagsActions.loadTags());
  }

  ngOnDestroy() {
    this.hubService.disconnect();
  }

  getsiblingIds(group: BoardGroup, groups: BoardGroup[]): string[] {
    return groups
      .filter((item) => item.id !== group.id)
      .map((item) => item.id.toString());
  }

  drop(event: CdkDragDrop<BoardGroup[]>) {
    moveItemInArray(
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );

    const groups = event.container.data;

    const prevGroup = groups[event.currentIndex - 1];
    const nextGroup = groups[event.currentIndex + 1];

    const preOrder = prevGroup && prevGroup.sortOrder;
    const nextOrder = nextGroup && nextGroup.sortOrder;

    const order = getNewSortOrder(preOrder, nextOrder);

    const { data } = event.item;

    if (data.sortOrder === order) {
      return;
    }

    this.moveBoardGroup(data, order);
  }

  moveBoardGroup(boardGroup: BoardGroup, sortOrder: number) {
    this.store.dispatch(
      GroupActions.editBoardGroup({
        boardGroup: {
          ...boardGroup,
          sortOrder,
        },
      })
    );
  }

  trackBoardGroup(_: number, group: BoardGroup) {
    return group?.id;
  }

  onDeleteGroupClicked(boardGroup: BoardGroup) {
    this.store.dispatch(GroupActions.deleteBoardGroup({ boardGroup }));
  }

  onGroupNameSubmitted(value: Event | string, group: BoardGroup) {
    if (value instanceof Event) return;

    const boardGroup: BoardGroup = {
      ...group,
      name: value,
    };

    this.store.dispatch(GroupActions.editBoardGroup({ boardGroup }));
  }

  onImportTasksClicked() {
    this.importTasksInput?.nativeElement.click();
  }

  onDeleteBoardClicked() {
    const boardId = this.board.id;

    if (boardId === undefined || boardId === null) return;

    this.store.dispatch(BoardActions.deleteBoard({ boardId }));
  }

  handleFileInput(event: Event) {
    const files = (event?.target as EventTarget & { files: File[] })?.files;

    if (!files || !files.length) return;

    const file = files[0];

    const boardIdentifier = this.board.identifier;

    this.store.dispatch(TaskActions.importTasks({ boardIdentifier, file }));
  }
}
