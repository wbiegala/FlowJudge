export interface DataGridRow {
  id: string;
}

export type DataGridCellValue =
  | string
  | number
  | Date
  | boolean
  | TwoLinesCell
  | IconCell
  | null
  | undefined;

export type DataGridColumnTextAlign = 'left' | 'center' | 'right';

export interface DataGridColumn<TRow extends DataGridRow> {
  id: string;
  name: string;
  headerTranslationKey: string;
  cell: (row: TRow) => DataGridCellValue;
  isVisible: boolean;
  isSortable: boolean;
  textAlign?: DataGridColumnTextAlign;
}

export interface TwoLinesCell {
  firstLine: string;
  secondLine: string;
}

export interface IconCell {
  icon: string;
  valueTranslationKey: string;
  isSvgIcon?: boolean;
  text?: string;
}

export interface DataGridAction {
  name: string;
  nameTranslationKey: string;
  icon: string;
  canExecute: () => boolean;
}

export interface DataGridRowAction<TRow extends DataGridRow> {
  name: string;
  nameTranslationKey: string;
  icon: string;
  canExecute: (row: TRow) => boolean;
}

export interface DataGridActionEvent {
  name: string;
}

export interface DataGridRowActionEvent {
  name: string;
  id: string;
}

export interface EmptyGridBehavior {
  messageTranslationKey: string;
  actionName: string;
}
