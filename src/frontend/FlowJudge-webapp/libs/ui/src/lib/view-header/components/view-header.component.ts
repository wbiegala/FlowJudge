import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ViewHeaderAction, ViewHeaderEvent } from '../view-header.model';
import { TranslatePipe } from '@ngx-translate/core';



@Component({
  selector: 'lib-view-header',
  imports: [ MatButtonModule, MatIconModule, MatTooltipModule, TranslatePipe ],
  templateUrl: './view-header.component.html',
  styleUrl: './view-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ViewHeaderComponent {
  title = input.required<string>();
  actions = input<Array<ViewHeaderAction>>([]);
  headerEvent = output<ViewHeaderEvent>();

  onActionClick(action: ViewHeaderAction) {
    this.headerEvent.emit({ actionName: action.name });
  }
}
