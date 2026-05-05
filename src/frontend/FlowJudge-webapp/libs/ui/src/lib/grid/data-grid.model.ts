export interface DataGridRow {
  id: string;
}

export interface DataGridColumn<TRow extends DataGridRow> {
  id: string;
  name: string;
  headerTranslationKey: string;
  cell: (row: TRow) => string | number | Date | boolean | null | undefined;
  isVisible: boolean;
  isSortable: boolean;
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
