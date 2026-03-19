import { AuthenticationService } from '@flow-judge-webapp/auth';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-header-user-panel',
  imports: [MatButtonModule, TranslatePipe],
  templateUrl: './header-user-panel.component.html',
  styleUrl: './header-user-panel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderUserPanelComponent {
  #authenticationService = inject(AuthenticationService);

  onRegisterClick() {
    this.#authenticationService.registerAccount();
  }
}
