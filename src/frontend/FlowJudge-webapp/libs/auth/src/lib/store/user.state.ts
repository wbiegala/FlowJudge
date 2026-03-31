import { inject, Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, StateToken } from '@ngxs/store';
import { AuthenticationService } from '../authentication.service';
import { Authenticate, SetUserContext, StartLogin, TryRestoreUserContext } from './user.actions';
import { produce } from 'immer';
import { catchError, map, of, tap } from 'rxjs';

export interface UserStateModel {
  isAuthenticated: boolean | null;
  data: null | {
    id: string;
    name: string;
    email: string;
  };
  accessToken: string | null;
  identityToken: string | null;
};

export const defaultUserState: UserStateModel = {
  isAuthenticated: null,
  data: null,
  accessToken: null,
  identityToken: null,
};

export const USER_STATE_TOKEN = new StateToken<UserStateModel>('user');

@State<UserStateModel>({
  name: USER_STATE_TOKEN,
  defaults: defaultUserState
})
@Injectable()
export class UserState {
  #authenticationService = inject(AuthenticationService);

  @Selector()
  static accessToken(state: UserStateModel) {
    return state.accessToken;
  }

  @Selector()
  static isAuthenticated(state: UserStateModel) {
    return state.isAuthenticated;
  }

  @Action(StartLogin)
  loginUserAction() {
    return this.#authenticationService.login();
  }

  @Action(Authenticate)
  authenticateAction(ctx: StateContext<UserStateModel>, action: Authenticate) {
    ctx.setState(produce((draft) => {
      draft.accessToken = action.accessToken;
      draft.identityToken = action.identityToken
    }));
    ctx.dispatch(new SetUserContext());
  }

  @Action(SetUserContext)
  setUserContextAction(ctx: StateContext<UserStateModel>) {
    return this.#authenticationService.getUserData().pipe(
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

  @Action(TryRestoreUserContext)
  tryRestoreUserContextAction(ctx: StateContext<UserStateModel>) {
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
