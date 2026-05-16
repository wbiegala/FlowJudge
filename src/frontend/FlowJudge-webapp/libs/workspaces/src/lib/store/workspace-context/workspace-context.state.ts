import { Action, Selector, State, StateContext, StateToken, Store } from "@ngxs/store";
import { inject, Injectable } from "@angular/core";
import { CleanWorkspaceContext, SetWorkspaceContext } from "./workspace-context.actions";
import { WorkspacesService } from "../../workspaces.service";
import { NotificationService, ProgressService } from "@flow-judge-webapp/ui";
import { tap } from "rxjs";
import { produce } from "immer";
import { AuthenticationState } from "@flow-judge-webapp/auth";
import { WorkspaceRole } from "../../models/workspace-shared.model";

export interface WorkspaceContext {
  id: string;
  name: string;
  role: WorkspaceRole;
}

export interface WorkspaceContextStateModel {
  isSet: boolean;
  context: WorkspaceContext | null;
}

const defaultState: WorkspaceContextStateModel = {
  isSet: false,
  context: null
}

export const WORKSPACE_CONTEXT_STATE_TOKEN = new StateToken<WorkspaceContextStateModel>('workspaceContext');

@State<WorkspaceContextStateModel>({
  name: WORKSPACE_CONTEXT_STATE_TOKEN,
  defaults: defaultState
})
@Injectable()
export class WorkspaceContextState {
  #workspacesService = inject(WorkspacesService);
  #progressService = inject(ProgressService);
  #notificationService = inject(NotificationService);
  #store = inject(Store);

  @Selector()
  static isWorkspaceContext(state: WorkspaceContextStateModel) {
    return state.isSet;
  }

  @Selector()
  static workspaceContext(state: WorkspaceContextStateModel) {
    return state.context;
  }

  @Selector()
  static workspaceContextId(state: WorkspaceContextStateModel) {
    return state.context?.id;
  }

  @Action(SetWorkspaceContext)
  setWorkspaceContext(ctx: StateContext<WorkspaceContextStateModel>, action: SetWorkspaceContext) {
    const isAuthenticated = this.#store.selectSnapshot(AuthenticationState.isAuthenticated);
    if (!isAuthenticated) {
      return;
    }

    const currentUserId = this.#store.selectSnapshot(AuthenticationState.userData)?.id;

    return this.#progressService.runInProgressBar(() => this.#workspacesService.getWorkspace(action.workspaceId).pipe(
      tap(response => {
        const memberRole = response.members.find(m => m.member.userId === currentUserId)?.role;
        if (memberRole === undefined || memberRole === null) {
          this.#notificationService.showError('WORKSPACES.CONTEXT.NOTIFICATIONS.SET_ERROR_NO_ROLE');
          throw new Error('Authenticated user is not a member of the selected workspace.');
        }

        ctx.setState(produce(draft => {
          draft.isSet = true;
          draft.context = {
            id: response.id,
            name: response.name,
            role: memberRole,
          };
        }));
        if (action.options.showSuccessNotification !== false) {
          this.#notificationService.showInfo('WORKSPACES.CONTEXT.NOTIFICATIONS.SET_SUCCESS', { workspaceName: response.name });
        }
      })
    ));
  }


  @Action(CleanWorkspaceContext)
  cleanWorkspaceContext(ctx: StateContext<WorkspaceContextStateModel>) {
    ctx.setState(defaultState);
  }
}
