import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'lib-no-access',
  imports: [TranslatePipe],
  template: `
  <h1>{{ 'AUTH.NO_ACCESS.NO_ACCESS_HEADER' | translate }}</h1>
  <p>{{ 'AUTH.NO_ACCESS.NO_ACCESS_CONTENT' | translate }}</p>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NoAccessComponent {}
