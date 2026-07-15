import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { produce } from 'immer';
import { tap } from 'rxjs';
import { PageSize } from '@flow-judge-webapp/ui';
import { MapGridDtoToModel } from '../../mappers/dto-model.mapper';
import { RepositoryGridItem } from '../../models/repository-grid-item.model';
import { RepositoriesService } from '../../repositories.service';
import { LoadRepositoriesGridItems } from './repositories-grid.actions';

export interface RepositoriesGridStateModel {
  pageSize: PageSize;
  pageNumber: number;
  totalCount: number;
  items: Array<RepositoryGridItem>;
  isLoading: boolean;
}

const defaultState: RepositoriesGridStateModel = {
  pageSize: 25,
  pageNumber: 1,
  totalCount: 0,
  items: [],
  isLoading: false,
};

export const REPOSITORIES_GRID_STATE_TOKEN = new StateToken<RepositoriesGridStateModel>('repositoriesGrid');

@State<RepositoriesGridStateModel>({
  name: REPOSITORIES_GRID_STATE_TOKEN,
  defaults: defaultState,
})
@Injectable()
export class RepositoriesGridState {
  #repositoriesService = inject(RepositoriesService);

  @Selector()
  static pageSize(state: RepositoriesGridStateModel) { return state.pageSize; }

  @Selector()
  static pageNumber(state: RepositoriesGridStateModel) { return state.pageNumber; }

  @Selector()
  static totalCount(state: RepositoriesGridStateModel) { return state.totalCount; }

  @Selector()
  static items(state: RepositoriesGridStateModel) { return state.items; }

  @Selector()
  static isLoading(state: RepositoriesGridStateModel) { return state.isLoading; }

  @Action(LoadRepositoriesGridItems)
  loadRepositoriesGridItems(ctx: StateContext<RepositoriesGridStateModel>, action: LoadRepositoriesGridItems) {
    ctx.patchState({ isLoading: true });

    return this.#repositoriesService.getRepositoriesGridData(action.pageNumber, action.pageSize).pipe(
      tap(data => ctx.setState(produce(draft => {
        draft.totalCount = data.totalCount;
        draft.pageSize = data.pageSize as PageSize;
        draft.pageNumber = data.pageNumber;
        draft.items = data.items.map(MapGridDtoToModel);
        draft.isLoading = false;
      }))),
    );
  }
}
