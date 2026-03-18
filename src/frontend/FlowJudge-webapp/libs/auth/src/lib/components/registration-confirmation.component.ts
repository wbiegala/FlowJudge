import { TranslatePipe } from '@ngx-translate/core';
import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lib-registration-confirmation',
  imports: [TranslatePipe],
  template: `
    <p>{{ 'AUTH.REGISTRATION.REGISTRATION_CONFIRMATION_HEADER' | translate }}</p>
    <p>{{ 'AUTH.REGISTRATION.REGISTRATION_CONFIRMATION_CONTENT' | translate }}</p>`,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationConfirmationComponent {}
