import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'lib-logout',
  imports: [TranslatePipe],
  template: `
  <h1>{{ 'AUTH.LOGOUT.LOGOUT_CONFIRMATION_HEADER' | translate }}</h1>
  <p>{{ 'AUTH.LOGOUT.LOGOUT_CONFIRMATION_CONTENT' | translate }}</p>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogoutComponent {}
