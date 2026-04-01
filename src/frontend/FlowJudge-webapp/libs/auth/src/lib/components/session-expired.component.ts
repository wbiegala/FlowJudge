import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'lib-session-expired',
  imports: [TranslatePipe],
  template: `
  <h1>{{ 'AUTH.SESSION_EXPIRED.SESSION_EXPIRED_HEADER' | translate }}</h1>
  <p>{{ 'AUTH.SESSION_EXPIRED.SESSION_EXPIRED_CONTENT' | translate }}</p>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionExpiredComponent {}
