import { MatInputModule } from '@angular/material/input';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { ControlErrorDirective, LoadingComponent, ProgressService, ViewHeaderComponent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';
import { MatStepperModule} from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { GitHubIntegrationsService } from '../../../github-integrations.service';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, finalize, map, switchMap, tap } from 'rxjs';

@Component({
  selector: 'lib-github-setup-wizard',
  imports: [ MatButtonModule, MatStepperModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, ViewHeaderComponent, TranslatePipe, ControlErrorDirective, LoadingComponent ],
  templateUrl: './github-setup-wizard.component.html',
  styleUrl: './github-setup-wizard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GitHubSetupWizardComponent {
  #activatedRoute = inject(ActivatedRoute);
  #progressService = inject(ProgressService);
  #githubIntegrationService = inject(GitHubIntegrationsService);

  isLoading = signal(true);

  readonly repositories = toSignal(this.#activatedRoute.params.pipe(
    map(params => params['installationStateId'] as string | undefined),
    filter((installationStateId): installationStateId is string =>
      installationStateId !== undefined &&
      installationStateId !== null &&
      installationStateId.trim() !== ''
    ),
    tap(() => this.isLoading.set(true)),
    switchMap(installationStateId =>
      this.#githubIntegrationService
        .getGitHubInstallationRepositories(installationStateId)
        .pipe(
          finalize(() => this.isLoading.set(false))
        )
    )
  ), { initialValue: [] });
}
