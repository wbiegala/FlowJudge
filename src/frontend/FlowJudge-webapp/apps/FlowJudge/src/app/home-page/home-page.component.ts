import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { AuthenticationState } from '@flow-judge-webapp/auth';
import { DialogService, NotificationService, ProgressService, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { UserLegalService } from '@flow-judge-webapp/user';
import { WorkspaceContextState } from '@flow-judge-webapp/workspaces';
import { Navigate } from '@ngxs/router-plugin';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-home-page',
  imports: [ MatButtonModule, ViewHeaderComponent ],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePageComponent {
  #store = inject(Store);
  #isAuthenticated = this.#store.selectSignal(AuthenticationState.isAuthenticated);
  #isWorkspaceContext = this.#store.selectSignal(WorkspaceContextState.isWorkspaceContext);
  notificationService = inject(NotificationService);
  dialogService = inject(DialogService);
  userLegalService = inject(UserLegalService);
  #progressService = inject(ProgressService);

  redirectEffect = effect(() => {
    if (this.#isAuthenticated() === null || this.#isAuthenticated() === false) {
      return;
    }

    if (this.#isWorkspaceContext() === false) {
      this.#store.dispatch(new Navigate(['workspaces']));
    }
  });

  onGetAss() {
    this.#progressService.runInProgressBar(() => this.userLegalService.getUserAss()).subscribe();
  }

  onErrorDialogClick() {
    this.dialogService.showErrorDialog(
      'wszystko poszło źle i chuj',
      'A konkretnie to w ogóle wszystko',
      [ 'Szyny były złe', 'I tory też były złe' ]
    );
  }
}
