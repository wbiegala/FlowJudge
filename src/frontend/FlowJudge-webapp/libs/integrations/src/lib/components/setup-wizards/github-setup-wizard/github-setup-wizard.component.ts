import { MatInputModule } from '@angular/material/input';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ControlErrorDirective, NotificationService, ProgressService, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';
import {MatStepper, MatStepperModule} from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { IntegrationsService } from '../../../integrations.service';
import { CreateIntegrationRequest } from '../../../integrations.model';
import { WorkspaceNavigationService } from '@flow-judge-webapp/workspaces';

@Component({
  selector: 'lib-github-setup-wizard',
  imports: [ MatButtonModule, MatStepperModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, ViewHeaderComponent, TranslatePipe, ControlErrorDirective ],
  templateUrl: './github-setup-wizard.component.html',
  styleUrl: './github-setup-wizard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GitHubSetupWizardComponent {
  #integrationId = signal('');
  #progressService = inject(ProgressService);
  #integrationsService = inject(IntegrationsService);
  #notificationService = inject(NotificationService);
  #workspaceNavigationService = inject(WorkspaceNavigationService);
  #cdr = inject(ChangeDetectorRef);


  basicDataForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(128)] }),
  });

  onStepOneNextClick(stepper: MatStepper) {
    this.basicDataForm.markAllAsTouched();
    this.basicDataForm.updateValueAndValidity({ emitEvent: true });
    this.#cdr.markForCheck();

    if (this.basicDataForm.invalid) {
      return;
    }

    const request = {
      name: this.basicDataForm.controls.name.value,
    } satisfies CreateIntegrationRequest;

    this.#progressService.runInProgressBar(() => this.#integrationsService.createIntegration('GitHub', request))
      .subscribe(response => {
        this.#integrationId.set(response.id);
        stepper.next();
      });
  }

  onStepTwoInstallClick() {
    this.#progressService.runInProgressBar(() => this.#integrationsService.connectIntegration(this.#integrationId()))
      .subscribe(response => {
        window.location.href = response.redirectUrl;
      });
  }
}
