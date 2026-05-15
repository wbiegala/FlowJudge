import { ChangeDetectionStrategy, Component } from '@angular/core';
import { DataGridComponent, PaginationComponent, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'lib-integration-grid',
  imports: [ DataGridComponent, PaginationComponent, ViewHeaderComponent, TranslatePipe ],
  templateUrl: './integration-grid.component.html',
  styleUrl: './integration-grid.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IntegrationGridComponent {
  readonly title = '';
}
