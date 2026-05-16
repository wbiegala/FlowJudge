import { Observable } from "rxjs";

export interface MainMenuItem {
  name: string;
  nameTranslationKey: string;
  tooltipTranslationKey?: string;
  icon: string;
  canDisplay: () => boolean;
  canClick: () => boolean;
  action: () => Observable<void>;
}
