import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MainMenuItem } from './main-menu.model';
import { Store } from '@ngxs/store';
import { WorkspaceContextState, WorkspaceNavigationService } from '@flow-judge-webapp/workspaces';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { of } from 'rxjs';
import { Navigate } from '@ngxs/router-plugin';

@Component({
  selector: 'app-main-menu',
  imports: [ MatButtonModule, MatIconModule, TranslatePipe ],
  templateUrl: './main-menu.component.html',
  styleUrl: './main-menu.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MainMenuComponent {
  #store = inject(Store);
  #isWorkspaceContext = this.#store.selectSignal(WorkspaceContextState.isWorkspaceContext);
  #workspaceContextId = this.#store.selectSignal(WorkspaceContextState.workspaceContextId);
  #workspaceNavigationService = inject(WorkspaceNavigationService);

  readonly items: Array<MainMenuItem> = [
    {
      name: 'integrations',
      nameTranslationKey: 'MAIN_MENU.BUTTONS.INTEGRATIONS.NAME',
      tooltipTranslationKey: 'MAIN_MENU.BUTTONS.INTEGRATIONS.TOOLTIP',
      icon: 'merge',
      canDisplay: () => this.#isWorkspaceContext(),
      canClick: () => true,
      action: () => this.#workspaceNavigationService.navigate(['integrations'])
    },
    {
      name: 'workspaces',
      nameTranslationKey: 'MAIN_MENU.BUTTONS.WORKSPACES.NAME',
      tooltipTranslationKey: 'MAIN_MENU.BUTTONS.WORKSPACES.TOOLTIP',
      icon: 'workspaces',
      canDisplay: () => true,
      canClick: () => true,
      action: () => this.#store.dispatch(new Navigate(['/workspaces']))
    },
    {
      name: 'userAccount',
      nameTranslationKey: 'MAIN_MENU.BUTTONS.USER_ACCOUNT.NAME',
      tooltipTranslationKey: 'MAIN_MENU.BUTTONS.USER_ACCOUNT.TOOLTIP',
      icon: 'person',
      canDisplay: () => true,
      canClick: () => true,
      action: () => of()
    }
  ]
}
