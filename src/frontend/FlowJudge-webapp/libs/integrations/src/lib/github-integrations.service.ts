import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL, CreatedResponse } from '@flow-judge-webapp/common';
import { Observable } from 'rxjs';
import { CommitGitHubIntegrationInstallationRequest, ConnectIntegrationResponse, GetGitHubInstallationRepositoriesResponseItem } from './github-integrations.model';

@Injectable({ providedIn: 'root' })
export class GitHubIntegrationsService {
  #httpClient = inject(HttpClient);
  #integrationsPathSegment = 'api/integrations/github' as const;
  #baseUrl = inject(API_BASE_URL);

  installGitHubApplication(): Observable<ConnectIntegrationResponse> {
    const url = `${this.#baseUrl}/${this.#integrationsPathSegment}/install`;

    return this.#httpClient.post<ConnectIntegrationResponse>(url, null);
  }

  getGitHubInstallationRepositories(installationStateId: string): Observable<Array<GetGitHubInstallationRepositoriesResponseItem>> {
    const url = `${this.#baseUrl}/${this.#integrationsPathSegment}/${installationStateId}/repositories`;

    return this.#httpClient.get<Array<GetGitHubInstallationRepositoriesResponseItem>>(url);
  }

  commitGitHubInstallation(installationStateId: string, request: CommitGitHubIntegrationInstallationRequest) {
    const url = `${this.#baseUrl}/${this.#integrationsPathSegment}/${installationStateId}/commit`;

    return this.#httpClient.post<CreatedResponse>(url, request);
  }
}
