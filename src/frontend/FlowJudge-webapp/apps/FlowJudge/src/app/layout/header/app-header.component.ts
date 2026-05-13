import { ChangeDetectionStrategy, Component, computed, inject, output, signal } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { HeaderProgressBarComponent } from './header-progress-bar/header-progress-bar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { HeaderUserPanelComponent } from './header-user-panel/header-user-panel.component';
import { RouterLink } from "@angular/router";
import { Store } from '@ngxs/store';
import { AuthenticationState } from '@flow-judge-webapp/auth';
import { HeaderWorkspacePanelComponent } from './header-workspace-panel/header-workspace-panel.component';

@Component({
  selector: 'app-header',
  imports: [HeaderProgressBarComponent, HeaderWorkspacePanelComponent, HeaderUserPanelComponent, MatToolbarModule, MatButtonModule, MatIconModule, RouterLink],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppHeaderComponent {
  #store = inject(Store);
  hideMenuButton = computed(() => {
    const isAuth = this.#store.selectSignal(AuthenticationState.isAuthenticated);
    const isLegal = signal(true);

    if (isAuth() === null || isAuth() === false) {
      return true;
    }

    if (isLegal() !== null && isLegal() === false) {
      return true;
    }

    return false;
  });

  menuClicked = output();
}
