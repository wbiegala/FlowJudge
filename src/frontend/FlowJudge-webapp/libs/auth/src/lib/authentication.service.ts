import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { API_BASE_URL } from '@flow-judge-webapp/common';
import { Observable } from "rxjs";
import { GetUserDataResponse, LogoutRequest, LogoutResponse, TokenPairResponse } from "./auth.model";

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  #httpClient = inject(HttpClient);
  #authPathSegment = 'api/auth' as const;
  #uiContextUrlParamName = 'uiContextUrl' as const;
  #baseUrl = inject(API_BASE_URL);

  registerAccount() {
    const clientOrigin = window.location.origin;
    const url = `${this.#baseUrl}/${this.#authPathSegment}/register?${this.#uiContextUrlParamName}=${encodeURIComponent(clientOrigin)}`;

    window.location.href = url;
  }

  login() {
    const currentUiContext = window.location.href;
    const url = `${this.#baseUrl}/${this.#authPathSegment}/login?${this.#uiContextUrlParamName}=${encodeURIComponent(currentUiContext)}`;

    window.location.href = url;
  }

  exchangeToken(stateId: string): Observable<TokenPairResponse> {
    const url = `/${this.#authPathSegment}/exchange-token?stateId=${stateId}`;

    return this.#httpClient.post<TokenPairResponse>(url, null);
  }

  refreshToken(): Observable<TokenPairResponse> {
    const url = `/${this.#authPathSegment}/refresh-token`;

    return this.#httpClient.post<TokenPairResponse>(url, null, { withCredentials: true });
  }

  getUserData(): Observable<GetUserDataResponse> {
    const url = `/${this.#authPathSegment}/me`;

    return this.#httpClient.get<GetUserDataResponse>(url);
  }

  logout(identityToken: string) {
    const url = `/${this.#authPathSegment}/logout`;
    const body = {
      identityToken: identityToken,
      uiContext: window.location.href
    } satisfies LogoutRequest;

    this.#httpClient.post<LogoutResponse>(url, body, { withCredentials: true })
      .subscribe(response => window.location.href = response.logoutRedirectUrl);
  }
}
