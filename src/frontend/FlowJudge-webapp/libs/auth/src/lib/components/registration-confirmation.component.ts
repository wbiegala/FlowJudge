import { TranslatePipe } from '@ngx-translate/core';
import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'lib-registration-confirmation',
  imports: [TranslatePipe],
  template: `
    <h1>{{ 'AUTH.REGISTRATION.REGISTRATION_CONFIRMATION_HEADER' | translate }}</h1>
    <p>{{ 'AUTH.REGISTRATION.REGISTRATION_CONFIRMATION_CONTENT' | translate }}</p>`,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegistrationConfirmationComponent {}
