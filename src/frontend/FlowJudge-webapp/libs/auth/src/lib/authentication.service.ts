import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { API_BASE_URL } from '@flow-judge-webapp/common';

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
}
