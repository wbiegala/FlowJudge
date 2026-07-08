import { inject, Injectable } from '@angular/core';
import { NotificationService, ProgressService, ViewMode } from '@flow-judge-webapp/ui';
import { Action, Selector, State, StateContext, StateToken, Store } from '@ngxs/store';
import { IntegrationDetails } from '../../models/integration-details.model';
import { IntegrationsService } from '../../integrations.service';
import { DisableTrackingForRepository, EnableTrackingForRepository, InitializeEditIntegration } from './integration-details.actions';
import { AuthenticationState } from '@flow-judge-webapp/auth';
import { tap } from 'rxjs';
import { produce } from 'immer';
import { MapToModel } from '../../mappers/dto-model.mapper';
import { WorkspaceContextState } from '@flow-judge-webapp/workspaces';

export interface BasicFormModel {
  name: string;
}

export interface IntegrationDetailsStateModel {
  viewMode: ViewMode;
  model: IntegrationDetails | null;
  basicForm: {
    model: BasicFormModel;
    dirty: boolean;
    status: string;
    errors: object;
  }
}

const defaultState: IntegrationDetailsStateModel = {
  viewMode: 'Preview',
  model: null,
  basicForm: {
    model: {
      name: ''
    },
    dirty: false,
    status: '',
    errors: {}
  }
}

export const INTEGRATION_DETAILS_STATE_TOKEN = new StateToken<IntegrationDetailsStateModel>('integration');

@State<IntegrationDetailsStateModel>({
  name: INTEGRATION_DETAILS_STATE_TOKEN,
  defaults: defaultState
})
@Injectable()
export class IntegrationDetailsState {
  #integrationsService = inject(IntegrationsService);
  #notificationService = inject(NotificationService);
  #progressService = inject(ProgressService);

  @Selector()
  static viewMode(state: IntegrationDetailsStateModel) {
    return state.viewMode;
  }

  @Selector()
  static repositories(state: IntegrationDetailsStateModel) {
    return state.model?.repositories ?? [];
  }

  @Action(InitializeEditIntegration)
  initializeEditIntegration(ctx: StateContext<IntegrationDetailsStateModel>, action: InitializeEditIntegration) {
    const currentWorkspace = inject(Store).selectSnapshot(WorkspaceContextState.workspaceContext);
    if (!currentWorkspace?.role) {
      return;
    }

    return this.#progressService.runInProgressBar(() => this.#integrationsService.getIntegrationDetails(action.integrationId)).pipe(
      tap(response => {
        ctx.setState(produce(draft => {
          draft.viewMode = currentWorkspace.role === 'Administrator' || currentWorkspace.role === 'Owner'
            ? 'Edit'
            : 'Preview';
          draft.model = MapToModel(response);
          draft.basicForm.model = { name: response.name }
        }));
      }),
    );
  }

  @Action(EnableTrackingForRepository)
  enableTrackingForRepository(ctx: StateContext<IntegrationDetailsStateModel>, action: EnableTrackingForRepository) {
    ctx.setState(produce(ctx.getState(), draft => {
      const repository = draft.model?.repositories.filter(repo => repo.id === action.repositoryId)[0];
      if (!repository) {
        return;
      }

      repository.trackingEnabled = true;
    }))
  }

  @Action(DisableTrackingForRepository)
  disableTrackingForRepository(ctx: StateContext<IntegrationDetailsStateModel>, action: DisableTrackingForRepository) {
    ctx.setState(produce(ctx.getState(), draft => {
      const repository = draft.model?.repositories.filter(repo => repo.id === action.repositoryId)[0];
      if (!repository) {
        return;
      }

      repository.trackingEnabled = false;
    }))
  }
}
