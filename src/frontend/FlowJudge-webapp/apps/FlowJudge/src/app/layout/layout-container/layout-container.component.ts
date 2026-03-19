import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';
import { AppHeaderComponent } from "../header/app-header.component";
import { MatSidenavModule } from '@angular/material/sidenav';
import { MainMenuComponent } from '../main-menu/main-menu.component';

@Component({
  selector: 'app-layout-container',
  imports: [AppHeaderComponent, MainMenuComponent, MatSidenavModule ],
  templateUrl: './layout-container.component.html',
  styleUrl: './layout-container.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LayoutContainerComponent {
  #mainMenuOpenStatus = signal(true);
  #showMainMenu = computed(() => {
    return true;
  });
  isMainMenu = computed(() => this.#showMainMenu() && this.#mainMenuOpenStatus());

  toggleMainMenu() {
    this.#mainMenuOpenStatus.update(value => !value);
  }
}
