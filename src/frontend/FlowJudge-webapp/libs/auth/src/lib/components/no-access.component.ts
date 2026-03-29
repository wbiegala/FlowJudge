import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lib-no-access',
  imports: [],
  template: `<p>NoAccessComponent works!</p>`,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NoAccessComponent {}
