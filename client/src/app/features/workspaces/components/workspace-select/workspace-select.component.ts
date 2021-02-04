import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { logout } from '@core/auth/store/auth.actions';
import { Workspace } from '@core/models/workspace';
import { filterObjectArray } from '@core/util/arrays';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { Store } from '@ngrx/store';
import { BehaviorSubject, fromEvent } from 'rxjs';
import { debounceTime, tap, throttleTime } from 'rxjs/operators';

@UntilDestroy()
@Component({
  selector: 'app-workspace-select',
  templateUrl: './workspace-select.component.html',
  styleUrls: ['./workspace-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkspaceSelectComponent implements OnInit, OnChanges {
  @ViewChild('dropdown') dropdownElementRef: ElementRef;

  @Input() options: Workspace[] = [];
  @Input() value: string;
  @Input() compact = false;

  @Output() selectChange = new EventEmitter<Workspace>();
  @Output() closed = new EventEmitter();

  searchControl = new FormControl();

  isOpen = false;
  currentWorkspace: Workspace;
  selected: Workspace;

  options$ = new BehaviorSubject<Workspace[]>([]);

  constructor(private store: Store) {}

  ngOnInit() {
    this.searchControl.valueChanges
      .pipe(debounceTime(300), untilDestroyed(this))
      .subscribe((term) => this.search(term));

    fromEvent(document, 'mousedown', {
      passive: true,
    })
      .pipe(
        untilDestroyed(this),
        throttleTime(200),
        tap(this.handleDocumentClick.bind(this))
      )
      .subscribe();

    fromEvent(document, 'keydown', {
      passive: true,
    })
      .pipe(untilDestroyed(this), tap(this.handleKeyDown.bind(this)))
      .subscribe();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.value || changes.options) {
      if (this.value) {
        const option = this.options.find((opt) => opt.slug === this.value);
        this.select(option);
      }

      this.resetOptions();
    }
  }

  resetOptions() {
    this.options$.next(
      this.options?.filter((opt) => opt.slug !== this.currentWorkspace.slug) ??
        []
    );
  }

  handleDocumentClick(event: Event) {
    if (!this.dropdownElementRef.nativeElement.contains(event.target)) {
      this.close();
    }
  }

  handleKeyDown(event: KeyboardEvent) {
    switch (event.key) {
      case 'ArrowUp':
        this.selectPreviousOption();
        break;
      case 'ArrowDown':
        this.selectNextOptiom();
        break;
      case 'Enter':
        this.select();
        break;
    }
  }

  selectNextOptiom() {
    const options = this.options$.value;

    if (!this.selected) {
      this.selected = options.length && options[0];
    } else {
      const currentIndex = options.findIndex(
        (opt) => opt.id === this.selected.id
      );

      if (options.length === currentIndex + 1) {
        return;
      }

      this.selected = options[currentIndex + 1];
    }

    this.options$.next(options);
  }

  selectPreviousOption() {
    const options = this.options$.value;

    if (!this.selected) {
      this.selected = options.length && options[0];
    } else {
      const currentIndex = options.findIndex(
        (opt) => opt.id === this.selected.id
      );

      if (currentIndex === 0) return;

      this.selected = this.options[currentIndex - 1];
    }

    this.options$.next(options);
  }

  open(dropdown: HTMLElement, origin: HTMLElement) {
    this.isOpen = true;
    if (this.compact) {
      dropdown.style.width = '200px';
      dropdown.style.left = '80px';
      dropdown.style.top = '0px';
    } else {
      dropdown.style.width = `${origin.offsetWidth}px`;
      dropdown.style.left = '0';
      dropdown.style.top = '55px';
    }
  }

  close() {
    this.closed.emit();
    this.isOpen = false;
    this.searchControl.patchValue('');

    this.resetOptions();
  }

  select(option: Workspace = null) {
    this.selected = option ?? this.selected;
    this.currentWorkspace = this.selected;
    this.selectChange.emit(this.selected);
    this.close();
    this.selected = null;

    this.resetOptions();
  }

  isActive(option: Workspace) {
    if (!this.selected) {
      return false;
    }
    return option.id === this.selected.id;
  }

  search(value: string) {
    if (!value) {
      this.options$.next(this.options);
    } else {
      this.options$.next(filterObjectArray(this.options, 'name', value));
      this.selectNextOptiom();
    }
  }

  onlogOutClicked() {
    this.close();
    this.store.dispatch(logout());
  }
}
