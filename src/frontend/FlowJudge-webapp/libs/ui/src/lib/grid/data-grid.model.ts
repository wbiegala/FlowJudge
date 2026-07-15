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

export interface DataGridColumn<TRow extends DataGridRow> {
  id: string;
  name: string;
  headerTranslationKey: string;
  cell: (row: TRow) => DataGridCellValue;
  isVisible: boolean;
  isSortable: boolean;
}

export interface TwoLinesCell {
  firstLine: string;
  secondLine: string;
}

export interface IconCell {
  icon: string;
  valueTranslationKey: string;
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
