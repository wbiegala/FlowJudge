import { ChangeDetectionStrategy, ChangeDetectorRef, Component, computed, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
  ControlErrorDirective,
  DataGridColumn,
  DataGridComponent,
  DataGridRowAction,
  DataGridRowActionEvent,
  EmptyGridBehavior,
  FormSectionComponent,
  getViewModeTranslationKey,
  IconCell,
  TwoLinesCell,
  ViewHeaderAction,
  ViewHeaderComponent,
  ViewHeaderEvent } from '@flow-judge-webapp/ui';
import { TranslatePipe } from '@ngx-translate/core';
import { NgxsFormDirective } from '@ngxs/form-plugin';
import { IntegrationDetailsState } from '../../store/integration-details/integration-details.state';
import { Store } from '@ngxs/store';
import { WorkspaceNavigationService } from '@flow-judge-webapp/workspaces';
import { IntegrationRepositoryDetails } from '../../models/integration-details.model';
import { DisableTrackingForRepository, EnableTrackingForRepository } from '../../store/integration-details/integration-details.actions';

@Component({
  selector: 'lib-integration-details-component',
  imports: [
    ViewHeaderComponent,
    FormSectionComponent,
    DataGridComponent,
    ControlErrorDirective,
    ReactiveFormsModule,
    TranslatePipe,
    NgxsFormDirective,
    MatInputModule,
    MatFormFieldModule ],
  templateUrl: './integration-details.component.html',
  styleUrl: './integration-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class IntegrationDetailsComponent {
  #store = inject(Store);
  #cdr = inject(ChangeDetectorRef);
  #workspaceNavigationService = inject(WorkspaceNavigationService);
  viewMode = this.#store.selectSignal(IntegrationDetailsState.viewMode);
  viewModeKey = computed(() => getViewModeTranslationKey(this.viewMode()));
  repositories = this.#store.selectSignal(IntegrationDetailsState.repositories);

  basicDataForm = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(128)] }),
  });

  readonly actions: Array<ViewHeaderAction> = [
    {
      name: 'save',
      nameTranslationKey: 'INTEGRATIONS.DETAILS.ACTIONS.SAVE',
      icon: 'check',
      canExecute: () => this.viewMode() !== 'Preview'
    },
    {
      name: 'exit',
      nameTranslationKey: 'INTEGRATIONS.DETAILS.ACTIONS.EXIT',
      icon: 'close',
      canExecute: () => true
    }
  ];

  handleActionEvent(event: ViewHeaderEvent) {
    switch (event.actionName) {
      case 'save': console.log('save'); break;
      case 'exit': this.#handleExitAction(); break;
    }
  }

  #handleExitAction() {
    this.#workspaceNavigationService.navigate(['integrations']);
  }

  readonly repositoriesGridColumns: Array<DataGridColumn<IntegrationRepositoryDetails>> = [
    {
      id: 'name',
      name: 'name',
      headerTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.COLUMNS.NAME',
      cell: row => ({ firstLine: row.name,  secondLine: row.fullName ?? '' } satisfies TwoLinesCell) ,
      isVisible: true,
      isSortable: true
    },
    {
      id: 'trackingEnabled',
      name: 'name',
      headerTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.COLUMNS.IS_TRACKING',
      cell: row => (
        {
          icon: row.trackingEnabled ? 'check' : 'block',
          valueTranslationKey: row.trackingEnabled ? 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.ENABLED' : 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.DISABLED'
        } satisfies IconCell
      ),
      isVisible: true,
      isSortable: true
    }
  ];

  readonly repositoriesGridRowActions: Array<DataGridRowAction<IntegrationRepositoryDetails>> = [
    {
      name: 'disableTracking',
      nameTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.ACTIONS.DISABLE_TRACKING',
      icon: 'keep_off',
      canExecute: row => this.viewMode() === 'Edit' && row.trackingEnabled
    },
    {
      name: 'enableTracking',
      nameTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.ACTIONS.ENABLE_TRACKING',
      icon: 'keep',
      canExecute: row => this.viewMode() === 'Edit' &&  !row.trackingEnabled
    },
    {
      name: 'goto',
      nameTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.ACTIONS.GO_TO',
      icon: 'preview',
      canExecute: _ => true
    }
  ];

  readonly repositoriesGridEmptyBehavior: EmptyGridBehavior = {
    messageTranslationKey: 'INTEGRATIONS.DETAILS.FORMS.REPOSITORIES.GRID.EMPTY_MESSAGE',
    actionName: ''
  }

  handleRepositoryRowAction(event: DataGridRowActionEvent) {
    switch (event.name) {
      case 'disableTracking': this.#disableTracking(event.id); break;
      case 'enableTracking': this.#enableTacking(event.id); break;
      case 'goto': this.#goToRepository(event.id); break;
    };
  }

  #enableTacking(id: string) {
    this.#store.dispatch(new EnableTrackingForRepository(id));
  }

  #disableTracking(id: string) {
    this.#store.dispatch(new DisableTrackingForRepository(id));
  }

  #goToRepository(id: string) {
    console.log('go to repository ' + id);
  }
}
