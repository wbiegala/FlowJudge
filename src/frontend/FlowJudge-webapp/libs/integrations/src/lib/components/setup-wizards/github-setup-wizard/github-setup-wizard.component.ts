
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, effect, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { filter, finalize, map, switchMap, tap } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import {
  ControlErrorDirective,
  FormSectionComponent,
  LoadingComponent,
  NotificationService,
  ProgressService,
  ViewHeaderAction,
  ViewHeaderComponent,
  ViewHeaderEvent } from '@flow-judge-webapp/ui';
  import { WorkspaceNavigationService } from '@flow-judge-webapp/workspaces';
import { GitHubIntegrationsService } from '../../../github-integrations.service';
import { GithubSetupRepositoryConfiguration } from './github-setup-wizard.model';
import { CommitGitHubIntegrationInstallationRequest } from '../../../github-integrations.model';


@Component({
  selector: 'lib-github-setup-wizard',
  imports: [
    MatButtonModule,
    MatSlideToggleModule,
    MatDividerModule,
    FormSectionComponent,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    ViewHeaderComponent,
    TranslatePipe,
    ControlErrorDirective,
    LoadingComponent,
  ],
  templateUrl: './github-setup-wizard.component.html',
  styleUrl: './github-setup-wizard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GitHubSetupWizardComponent {
  #cdr = inject(ChangeDetectorRef);
  #activatedRoute = inject(ActivatedRoute);
  #progressService = inject(ProgressService);
  #githubIntegrationService = inject(GitHubIntegrationsService);
  #workspaceNavigationService = inject(WorkspaceNavigationService);
  #notificationService = inject(NotificationService);
  #installationStateId = signal('');

  constructor() {
    effect(() => {
      this.repositoryData.set(this.#repositories());
    });
  }

  isLoading = signal(true);
  readonly repositoryData = signal<GithubSetupRepositoryConfiguration[]>([]);


  readonly actions: Array<ViewHeaderAction> = [
    {
      name: 'commit',
      nameTranslationKey: 'INTEGRATIONS.SETUP-WIZARD.GITHUB.ACTIONS.COMMIT',
      icon: 'check',
      canExecute: () => this.isLoading() === false
    }
  ];

  #repositories = toSignal(
    this.#activatedRoute.params.pipe(
      map(params => params['installationStateId'] as string | undefined),
      filter((installationStateId): installationStateId is string =>
        installationStateId !== undefined &&
        installationStateId !== null &&
        installationStateId.trim() !== ''
      ),
      tap(installationStateId => { this.isLoading.set(true); this.#installationStateId.set(installationStateId);}),
      switchMap(installationStateId =>
        this.#githubIntegrationService
          .getGitHubInstallationRepositories(installationStateId)
          .pipe(
            map(repos =>
              repos.map(repo => ({
                githubId: repo.id,
                name: repo.name,
                fullName: repo.fullName,
                isEnabled: true,
              }) satisfies GithubSetupRepositoryConfiguration)
            ),
            finalize(() => this.isLoading.set(false))
          )
      )
    ),
    { initialValue: [] }
  );

  basicDataForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(128)] }),
  });

  onEnabledChanged(repoId: number, checked: boolean): void {
    this.repositoryData.update(repositories =>
      repositories.map(repo =>
        repo.githubId === repoId
          ? { ...repo, isEnabled: checked }
          : repo
      )
    );
  }

  onHeaderEvent(event: ViewHeaderEvent) {
    switch (event.actionName) {
      case 'commit': this.#onCommitClick(); break;
    }
  }

  #onCommitClick() {
    this.basicDataForm.markAllAsTouched();
    this.basicDataForm.updateValueAndValidity({ emitEvent: true });
    this.#cdr.markForCheck();

    if (this.basicDataForm.invalid) {
      return;
    }

    const dto = {
      name: this.basicDataForm.controls.name.value,
      repositoriesConfiguration: this.repositoryData().map(r => {
        return {
          githubId: r.githubId,
          track: r.isEnabled
        };
      }),
    } satisfies CommitGitHubIntegrationInstallationRequest;
    const installationStateId = this.#installationStateId();

    this.#progressService
      .runInProgressBar(() => this.#githubIntegrationService.commitGitHubInstallation(installationStateId, dto))
      .subscribe(response => {
        this.#notificationService.showSuccess('INTEGRATIONS.SETUP-WIZARD.GITHUB.RESULTS.SUCCESS', { integrationName: dto.name })
        this.#workspaceNavigationService.navigate(['/integrations']);
      });
  }
}
