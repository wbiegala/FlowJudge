import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lib-session-expired',
  imports: [],
  template: `<p>SessionExpiredComponent works!</p>`,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionExpiredComponent {}
