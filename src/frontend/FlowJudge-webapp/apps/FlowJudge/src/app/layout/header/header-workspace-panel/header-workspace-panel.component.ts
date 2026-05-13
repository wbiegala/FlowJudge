import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { WorkspaceContextState } from '@flow-judge-webapp/workspaces';
import { TranslatePipe } from '@ngx-translate/core';
import { Navigate } from '@ngxs/router-plugin';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-header-workspace-panel',
  imports: [ MatButtonModule, MatIconModule, TranslatePipe, MatTooltipModule ],
  templateUrl: './header-workspace-panel.component.html',
  styleUrl: './header-workspace-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderWorkspacePanelComponent {
  #store = inject(Store);
  isWorkspaceContext = this.#store.selectSignal(WorkspaceContextState.isWorkspaceContext);
  workspaceContext = this.#store.selectSignal(WorkspaceContextState.workspaceContext);

  onClick() {
    this.#store.dispatch(new Navigate(['workspaces']))
  }

}
