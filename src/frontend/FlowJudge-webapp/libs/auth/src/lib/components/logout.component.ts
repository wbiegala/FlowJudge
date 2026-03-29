import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lib-logout',
  imports: [],
  template: `<p>LogoutComponent works!</p>`,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogoutComponent {}
