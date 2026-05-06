export type PaginationEventType = 'PageChanged' | 'PageSizeChanged';

export type PageSize = 10 | 25 | 50 | 100 | 1000;

export interface PaginationEvent {
  type: PaginationEventType;
  value: number;
}
