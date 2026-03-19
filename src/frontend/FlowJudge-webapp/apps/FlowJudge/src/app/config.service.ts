import { inject, Injectable } from '@angular/core';
import { APP_CONFIG, AppConfig } from '../main';

@Injectable({ providedIn: 'root' })
export class ConfigService {
  private readonly cfg = inject(APP_CONFIG);
  get apiUrl(): string { return this.cfg.apiUrl; }
  get raw(): AppConfig { return this.cfg; }
}
