import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { AuthenticationService } from '../authentication.service';
import { Authenticate, SetTenantContext, StartLogin, TryRestoreTenantContext } from './tenant.actions';
import { produce } from 'immer';
import { catchError, map, of, switchMap, tap } from 'rxjs';

export interface TenantStateModel {
  isAuthenticated: boolean | null;
  data: null | {
    id: string;
    name: string;
    email: string;
  };
  accessToken: string | null;
  identityToken: string | null;
};

export const defaultTenantState: TenantStateModel = {
  isAuthenticated: null,
  data: null,
  accessToken: null,
  identityToken: null,
};

export const TENANT_STATE_TOKEN = new StateToken<TenantStateModel>('tenant');

@State<TenantStateModel>({
  name: TENANT_STATE_TOKEN,
  defaults: defaultTenantState
})
@Injectable()
export class TenantState {
  #authenticationService = inject(AuthenticationService);

  @Selector()
  static accessToken(state: TenantStateModel) {
    return state.accessToken;
  }

  @Selector()
  static isAuthenticated(state: TenantStateModel) {
    return state.isAuthenticated;
  }

  @Action(StartLogin)
  loginUserAction() {
    return this.#authenticationService.login();
  }

  @Action(Authenticate)
  authenticateAction(ctx: StateContext<TenantStateModel>, action: Authenticate) {
    ctx.setState(produce((draft) => {
      draft.accessToken = action.accessToken;
      draft.identityToken = action.identityToken
    }));
    ctx.dispatch(new SetTenantContext());
  }

  @Action(SetTenantContext)
  setTenantContextAction(ctx: StateContext<TenantStateModel>) {
    return this.#authenticationService.getTenantData().pipe(
      tap(response => ctx.setState(produce((draft) => {
        draft.isAuthenticated = true;
        draft.data = {
          id: response.id,
          name: response.username,
          email: response.email,
        };
      }))),
      map(response => response.id),
    );
  }

  @Action(TryRestoreTenantContext)
  tryRestoreUserContextAction(ctx: StateContext<TenantStateModel>) {
    return this.#authenticationService.refreshToken().pipe(
      tap(response => ctx.dispatch(new Authenticate(response.accessToken, response.identityToken))),
      catchError(() => {
        ctx.setState(produce(draft => {
          draft.isAuthenticated = false;
        }));
        return of();
      }));
  }
}
