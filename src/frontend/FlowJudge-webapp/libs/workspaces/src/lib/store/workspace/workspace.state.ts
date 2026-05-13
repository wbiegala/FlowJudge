import { AuthenticationState } from '@flow-judge-webapp/auth';
import { NotificationService, ProgressService, ViewMode } from "@flow-judge-webapp/ui";
import { Action, Selector, State, StateContext, StateToken, Store } from "@ngxs/store";
import { inject, Injectable } from "@angular/core";
import { WorkspaceDetails } from "../../models/workspace-details.model";
import { CleanWorkspace, InitializeEditWorkspace, InitializeNewWorkspace, SaveWorkspace } from "./workspace.actions";
import { produce } from "immer";
import { WorkspacesService } from "../../workspaces.service";
import { tap } from "rxjs";
import { mapToModel } from "../../mappers/dto-model.mapper";

interface BasicFormModel {
  name: string;
}

export interface WorkspaceStateModel {
  viewMode: ViewMode;
  model: WorkspaceDetails | null;
  basicForm: {
    model: BasicFormModel;
    dirty: boolean;
    status: string;
    errors: object;
  }
}

const defaultState: WorkspaceStateModel = {
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
    const currentUser = inject(Store).selectSnapshot(AuthenticationState.userData);
    return this.#progressService.runInProgressBar(() => this.#workspaceService.getWorkspace(action.workspaceId)).pipe(
      tap(response => {
        ctx.setState(produce(draft => {
          const model = mapToModel(response);
          const currentRole = response.members.filter(m => m.member.userId === currentUser?.id)[0].role;
          draft.viewMode = currentRole === 'Owner'
            ? 'Edit'
            : 'Preview';
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

    if (state.viewMode === 'Edit' && (state.model === null || state.model.id === null )) {
      return;
    }

    const saveAction = state.viewMode === 'Edit'
      ? this.#workspaceService.updateWorkspace(state.model?.id ?? '', state.basicForm.model.name)
      : this.#workspaceService.createWorkspace(state.basicForm.model.name);

    return this.#progressService.runInProgressBar(() => saveAction).pipe(
      tap(() => this.#notificationService.showSuccess('WORKSPACES.DETAILS.RESULTS.SAVE_SUCCESS')),
    );
  }

  @Action(CleanWorkspace)
  cleanWorkspace(ctx: StateContext<WorkspaceStateModel>) {
    ctx.setState(defaultState);
  }
}
