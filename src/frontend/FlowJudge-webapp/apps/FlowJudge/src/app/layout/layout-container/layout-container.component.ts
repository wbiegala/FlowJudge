import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { AppHeaderComponent } from "../header/app-header.component";
import { MatSidenavModule } from '@angular/material/sidenav';
import { MainMenuComponent } from '../main-menu/main-menu.component';
import { Store } from '@ngxs/store';
import { AuthenticationState } from '@flow-judge-webapp/auth';

@Component({
  selector: 'app-layout-container',
  imports: [AppHeaderComponent, MainMenuComponent, MatSidenavModule ],
  templateUrl: './layout-container.component.html',
  styleUrl: './layout-container.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutContainerComponent {
  #mainMenuOpenStatus = signal(true);
  #store = inject(Store);
  #showMainMenu = computed(() => {
    const isAuth = this.#store.selectSignal(AuthenticationState.isAuthenticated);
    const isLegal = signal(true);

    return isAuth() !== null && isAuth() === true && isLegal() !== null && isLegal() === true;
  });
  isMainMenu = computed(() => this.#showMainMenu() && this.#mainMenuOpenStatus());

  toggleMainMenu() {
    this.#mainMenuOpenStatus.update(value => !value);
  }
}
