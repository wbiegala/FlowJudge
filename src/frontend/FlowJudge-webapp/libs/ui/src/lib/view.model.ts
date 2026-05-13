export type ViewMode = 'New' | 'Preview' | 'Edit';

export function getViewModeTranslationKey(mode: ViewMode): string {
  switch (mode) {
    case 'New':
      return 'UI.VIEW_MODE.NEW';
    case 'Edit':
      return 'UI.VIEW_MODE.EDIT';
    case 'Preview':
      return 'UI.VIEW_MODE.PREVIEW';
  }
}
