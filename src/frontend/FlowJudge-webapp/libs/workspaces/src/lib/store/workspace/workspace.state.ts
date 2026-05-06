import { NotificationService, ProgressService, ViewMode } from "@flow-judge-webapp/ui";
import { Action, Selector, State, StateContext, StateToken, Store } from "@ngxs/store";
import { inject, Injectable } from "@angular/core";
import { WorkspaceDetails } from "../../models/workspace-details.model";
import { InitializeEditWorkspace, InitializeNewWorkspace, SaveWorkspace } from "./workspace.actions";
import { produce } from "immer";
import { WorkspacesService } from "../../workspaces.service";
import { tap } from "rxjs";
import { Navigate } from "@ngxs/router-plugin";
import { mapToModel } from "../../mappers/dto-model.mapper";

export interface WorkspaceStateModel {
  viewMode: ViewMode;
  model?: WorkspaceDetails;
  basicForm: {
    model: {
      name: string;
    };
    dirty: boolean;
    status: string;
    errors: any;
  }
}

const defaultState: WorkspaceStateModel = {
  viewMode: 'Preview',
  model: undefined,
  basicForm: {
    model: {
      name: ''
    },
    dirty: false,
    status: '',
    errors: {}
  }
}

export const WORKSPACE_STATE_TOKEN = new StateToken<WorkspaceStateModel>('workspace');

@State<WorkspaceStateModel>({
  name: WORKSPACE_STATE_TOKEN,
  defaults: defaultState
})
@Injectable()
export class WorkspaceState {
  #workspaceService = inject(WorkspacesService);
  #notificationService = inject(NotificationService);
  #progressService = inject(ProgressService);
  #store = inject(Store);

  @Selector()
  static viewMode(state: WorkspaceStateModel) {
    return state.viewMode;
  }

  @Action(InitializeNewWorkspace)
  initializeNewWorkspace(ctx: StateContext<WorkspaceStateModel>) {
    ctx.setState(produce(draft => {
      draft.viewMode = 'New';
    }));
  }

  @Action(InitializeEditWorkspace)
  initializeEditWorkspace(ctx: StateContext<WorkspaceStateModel>, action: InitializeEditWorkspace) {
    return this.#progressService.runInProgressBar(() => this.#workspaceService.getWorkspace(action.workspaceId)).pipe(
      tap(response => {
        ctx.setState(produce(draft => {
          const model = mapToModel(response);
          draft.viewMode = 'Edit';
          draft.model = model;
          draft.basicForm.model = {
            name: model.name
          };
        }));
      }),
    );
  }

  @Action(SaveWorkspace)
  saveWorkspace(ctx: StateContext<WorkspaceStateModel>) {
    const state = ctx.getState();
    if (state.viewMode === 'Preview') {
      return;
    }

    //only new for now...
    return this.#progressService.runInProgressBar(() => this.#workspaceService.createWorkspace(state.basicForm.model.name)).pipe(
      tap(_ => this.#notificationService.showSuccess('WORKSPACES.DETAILS.RESULTS.SAVE_SUCCESS')),
    );
  }
}
