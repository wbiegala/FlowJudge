import { AuthenticationService, StartLogin, StartLogout, AuthenticationState } from '@flow-judge-webapp/auth';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { ProgressService, ViewportService } from '@flow-judge-webapp/ui';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-header-user-panel',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, TranslatePipe, MatTooltipModule],
  templateUrl: './header-user-panel.component.html',
  styleUrl: './header-user-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderUserPanelComponent {
  #authService = inject(AuthenticationService);
  #progressService = inject(ProgressService);
  #store = inject(Store);
  isViewportNarrow = inject(ViewportService).isNarrow;
  isAuthenticated = this.#store.selectSignal(AuthenticationState.isAuthenticated);
  userData = this.#store.selectSignal(AuthenticationState.userData);

  onRegisterClick() {
    this.#progressService.start();
    this.#authService.registerAccount();
  }

  onLoginClick() {
    this.#store.dispatch(new StartLogin());
  }

  onLogoutClick() {
    this.#store.dispatch(new StartLogout());
  }
}
