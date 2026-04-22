import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { DialogService, NotificationService } from '@flow-judge-webapp/ui';
import { UserLegalService } from '@flow-judge-webapp/user';

@Component({
  selector: 'app-home-page',
  imports: [MatButtonModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePageComponent {
  notificationService = inject(NotificationService);
  dialogService = inject(DialogService);
  userLegalService = inject(UserLegalService);

  onGetAss() {
    this.userLegalService.getUserAss().subscribe();
  }

  onErrorDialogClick() {
    this.dialogService.showErrorDialog(
      'wszystko poszło źle i chuj',
      'A konkretnie to w ogóle wszystko',
      [ 'Szyny były złe', 'I tory też były złe' ]
    );
  }
}
