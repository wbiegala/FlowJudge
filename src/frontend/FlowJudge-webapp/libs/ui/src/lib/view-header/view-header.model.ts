export interface ViewHeaderAction {
  name: string;
  nameTranslationKey: string;
  icon: string;
  canExecute: () => boolean;
}

export interface ViewHeaderEvent {
  actionName: string;
}

export interface ViewHeaderIcon {
  icon: string;
  isSvgIcon: boolean;
  tooltipTranslationKey: string;
}
