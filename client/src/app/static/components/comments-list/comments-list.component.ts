import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { UserResponse } from '@core/auth/store/auth.models';
import { CommentViewModel } from '@core/models/comment';

@Component({
  selector: 'app-comments-list',
  templateUrl: './comments-list.component.html',
  styleUrls: ['./comments-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsListComponent {
  @Input() user: UserResponse;
  @Input() comments: CommentViewModel[];

  @Output() deleteComment = new EventEmitter<CommentViewModel>();
  @Output() commentSubmit = new EventEmitter<string>();

  formGroup = new FormGroup({
    comment: new FormControl(),
  });

  get comment() {
    return this.formGroup.get('comment');
  }

  get value() {
    return this.comment.value;
  }

  submit() {
    this.commentSubmit.emit(this.value);
    this.comment.reset();
  }
}
