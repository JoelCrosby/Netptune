import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  Input,
} from '@angular/core';
import { getColourForKey } from '@core/util/colors/color-util';

@Component({
  selector: 'app-avatar',
  templateUrl: './avatar.component.html',
  styleUrls: ['./avatar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AvatarComponent implements OnInit {
  @Input() name: string;
  @Input() size: string | number = '32';
  @Input() imageUrl: string;
  @Input() border = false;
  @Input() tooltip = true;
  @Input() borderRadius: string | number = '50%';

  backgroundColor: string;
  color = '#fff';

  get sizePx() {
    return this.size + 'px';
  }

  get fontSize() {
    if (typeof this.size === 'number') return this.size / 2;

    return Number.parseInt(this.size, 2) / 2;
  }

  constructor() {}

  ngOnInit() {
    this.backgroundColor = getColourForKey(this.name);
  }
}
