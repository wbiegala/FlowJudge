import { produce } from 'immer';
import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { PageSize } from '@flow-judge-webapp/ui';
import { IntegrationGridItem } from '../../models/integration-grid-item.model';
import { IntegrationsService } from '../../integrations.service';
import { LoadIntegrationsGridItems } from './integrations-grid.actions';
import { tap } from 'rxjs';
import { MapGridDtoToModel } from '../../mappers/dto-model.mapper';


export interface IntegrationsGridStateModel {
  pageSize: PageSize;
  pageNumber: number;
  totalCount: number;
  items: Array<IntegrationGridItem>;
  isLoading: boolean;
}

const defaultState: IntegrationsGridStateModel = {
  pageSize: 25,
  pageNumber: 1,
  totalCount: 0,
  items: [],
  isLoading: false,
}

export const INTEGRATIONS_GRID_STATE_TOKEN = new StateToken<IntegrationsGridStateModel>('integrationsGrid');

@State<IntegrationsGridStateModel>({
  name: INTEGRATIONS_GRID_STATE_TOKEN,
  defaults: defaultState
})
@Injectable()
export class IntegrationsGridState {
  #integrationsService = inject(IntegrationsService);

  @Selector()
  static pageSize(state: IntegrationsGridStateModel) {
    return state.pageSize;
  }

  @Selector()
  static pageNumber(state: IntegrationsGridStateModel) {
    return state.pageNumber;
  }

  @Selector()
  static totalCount(state: IntegrationsGridStateModel) {
    return state.totalCount;
  }

  @Selector()
  static items(state: IntegrationsGridStateModel) {
    return state.items;
  }

  @Selector()
  static isLoading(state: IntegrationsGridStateModel) {
    return state.isLoading;
  }

  @Action(LoadIntegrationsGridItems)
  loadIntegrationsGridItems(ctx: StateContext<IntegrationsGridStateModel>, action: LoadIntegrationsGridItems) {
    ctx.patchState({isLoading: true});
    return this.#integrationsService.getIntegrationsGridData(action.pageNumber, action.pageSize).pipe(
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
