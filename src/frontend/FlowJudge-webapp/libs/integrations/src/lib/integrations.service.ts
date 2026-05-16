import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL, CreatedResponse, PagedResult, PaginationQueryParams } from '@flow-judge-webapp/common';
import { CreateIntegrationRequest, GetIntegrationsResponseItem } from './integrations.model';
import { IntegrationProvider } from './models/integration-shared.model';

@Injectable({ providedIn: 'root' })
export class IntegrationsService {
  #httpClient = inject(HttpClient);
  #integrationsPathSegment = 'api/integrations' as const;
  #baseUrl = inject(API_BASE_URL);

  createIntegration(provider: IntegrationProvider, data: CreateIntegrationRequest) {
    const url = `${this.#baseUrl}/${this.#integrationsPathSegment}/${provider}`;

    return this.#httpClient.post<CreatedResponse>(url, data);
  }

  getIntegrationsGridData(pageNumber: number, pageSize: number): Observable<PagedResult<GetIntegrationsResponseItem>> {
    const params = {
      pageNumber: pageNumber,
      pageSize: pageSize,
    } satisfies PaginationQueryParams;
    const url = `${this.#baseUrl}/${this.#integrationsPathSegment}`;

    return this.#httpClient.get<PagedResult<GetIntegrationsResponseItem>>(url, { params: params })
  }
}
