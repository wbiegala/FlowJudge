import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LayoutContainerComponent } from './layout-container/layout-container.component';

@Component({
  selector: 'app-routing-layout',
  imports: [RouterOutlet, LayoutContainerComponent],
  template: `
  <app-layout-container>
    <router-outlet></router-outlet>
  </app-layout-container>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoutingLayoutComponent {}
