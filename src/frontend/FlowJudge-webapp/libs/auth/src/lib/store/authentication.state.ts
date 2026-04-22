import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { AuthenticationService } from '../authentication.service';
import { produce } from 'immer';
import { catchError, map, of, tap } from 'rxjs';
import { Authenticate, ClearAuthenticatedUserContext, SetAuthenticatedUserContext, StartLogin, StartLogout, TryRestoreAuthenticatedUserContext } from './authentication.actions';
import { ProgressService } from '@flow-judge-webapp/ui';

export interface AuthenticationStateModel {
  isAuthenticated: boolean | null;
  data: null | {
    id: string;
    name: string;
    email: string;
  };
  accessToken: string | null;
  identityToken: string | null;
};

export const defaultAuthenticationState: AuthenticationStateModel = {
  isAuthenticated: null,
  data: null,
  accessToken: null,
  identityToken: null,
};

export const AUTHENTICATION_STATE_TOKEN = new StateToken<AuthenticationStateModel>('authentication');

@State<AuthenticationStateModel>({
  name: AUTHENTICATION_STATE_TOKEN,
  defaults: defaultAuthenticationState
})
@Injectable()
export class AuthenticationState {
  #authenticationService = inject(AuthenticationService);
  #progressService = inject(ProgressService);

  @Selector()
  static accessToken(state: AuthenticationStateModel) {
    return state.accessToken;
  }

  @Selector()
  static isAuthenticated(state: AuthenticationStateModel) {
    return state.isAuthenticated;
  }

  @Selector()
  static userData(state: AuthenticationStateModel) {
    return state.data === null
      ? null
      : { id: state.data.id, name: state.data.name, email: state.data.email };
  }

  @Action(StartLogin)
  loginAction() {
    this.#progressService.start();
    return this.#authenticationService.login();
  }

  @Action(Authenticate)
  authenticateAction(ctx: StateContext<AuthenticationStateModel>, action: Authenticate) {
    ctx.setState(produce((draft) => {
      draft.accessToken = action.accessToken;
      draft.identityToken = action.identityToken;
    }));
    ctx.dispatch(new SetAuthenticatedUserContext());
  }

  @Action(SetAuthenticatedUserContext)
  setAuthenticatedUserContextAction(ctx: StateContext<AuthenticationStateModel>) {
    return this.#progressService.runInProgressBar(() => this.#authenticationService.getUserData().pipe(
      tap(response => ctx.setState(produce((draft) => {
        draft.data = {
          id: response.id,
          name: response.username,
          email: response.email,
        };
        draft.isAuthenticated = true;
      }))),
      map(response => response.id),
    ));
  }

  @Action(TryRestoreAuthenticatedUserContext)
  tryRestoreAuthenticatedUserContextAction(ctx: StateContext<AuthenticationStateModel>) {
    return this.#progressService.runInProgressBar(() => this.#authenticationService.refreshToken().pipe(
      tap(response => ctx.dispatch(new Authenticate(response.accessToken, response.identityToken))),
      catchError(() => {
        ctx.setState(produce(draft => {
          draft.isAuthenticated = false;
        }));
        return of();
      })));
  }

  @Action(StartLogout)
  startLogoutAction(ctx: StateContext<AuthenticationStateModel>) {
    this.#progressService.start();
    const state = ctx.getState();

    if (!state.isAuthenticated || state.identityToken === null) {
      return;
    }

    return this.#authenticationService.logout(state.identityToken);
  }

  @Action(ClearAuthenticatedUserContext)
  clearAuthenticatedUserContextAction(ctx: StateContext<AuthenticationStateModel>) {
    ctx.setState(produce((draft) => {
      draft.isAuthenticated = false;
      draft.accessToken = null;
      draft.identityToken = null;
      draft.data = null;
    }));
  }
}
