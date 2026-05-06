import { produce } from 'immer';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { MapDtoToModel, WorkspaceGridItem } from '../../models/workspace-grid-item.model';
import { Injectable, inject } from '@angular/core';
import { WorkspacesService } from '../../workspaces.service';
import { InitializeWorkspacesGrid, LoadWorkspacesGridItems } from './workspaces-grid.actions';
import { tap } from 'rxjs';
import { PageSize } from '@flow-judge-webapp/ui';

export interface WorkspacesGridStateModel {
  pageSize: PageSize;
  pageNumber: number;
  totalCount: number;
  items: Array<WorkspaceGridItem>;
  isLoading: boolean;
}

const defaultState: WorkspacesGridStateModel = {
  pageSize: 25,
  pageNumber: 1,
  totalCount: 0,
  items: [],
  isLoading: false,
}

export const WORKSPACES_GRID_STATE_TOKEN = new StateToken<WorkspacesGridStateModel>('workspacesGrid');

@State<WorkspacesGridStateModel>({
  name: WORKSPACES_GRID_STATE_TOKEN,
  defaults: defaultState
})
@Injectable()
export class WorkspacesGridState {
  #workspacesService = inject(WorkspacesService);

  @Selector()
  static pageSize(state: WorkspacesGridStateModel) {
    return state.pageSize;
  }

  @Selector()
  static pageNumber(state: WorkspacesGridStateModel) {
    return state.pageNumber;
  }

  @Selector()
  static totalCount(state: WorkspacesGridStateModel) {
    return state.totalCount;
  }

  @Selector()
  static items(state: WorkspacesGridStateModel) {
    return state.items;
  }

  @Selector()
  static isLoading(state: WorkspacesGridStateModel) {
    return state.isLoading;
  }

  @Action(InitializeWorkspacesGrid)
  initializeWorkspacesGrid(ctx: StateContext<WorkspacesGridStateModel>) {
    ctx.setState(defaultState);
  }

  @Action(LoadWorkspacesGridItems)
  loadWorkspacesGridItems(ctx: StateContext<WorkspacesGridStateModel>, action: LoadWorkspacesGridItems) {
    ctx.patchState({isLoading: true});
    return this.#workspacesService.getWorkspacesGridData(action.pageNumber, action.pageSize).pipe(
      tap(data => ctx.setState(produce(draft => {
        draft.totalCount = data.totalCount;
        draft.pageSize = data.pageSize as PageSize;
        draft.pageNumber = data.pageNumber;
        draft.items = data.items.map(MapDtoToModel);
        draft.isLoading = false;
      })))
    )
  }
}
