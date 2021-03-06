import { Basemodel } from './basemodel';

export interface Board extends Basemodel {
  name: string;
  identifier: string;
  projectId: number;

  metaInfo?: BoardMeta;
}

export enum BoardType {
  userDefined = 0,
  default = 1,
}

export interface BoardMeta {
  color?: string;
}
