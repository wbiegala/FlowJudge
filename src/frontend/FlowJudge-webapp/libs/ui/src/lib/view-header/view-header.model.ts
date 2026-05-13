export interface ViewHeaderAction {
  name: string;
  nameTranslationKey: string;
  icon: string;
  canExecute: () => boolean;
}

export interface ViewHeaderEvent {
  actionName: string;
}
