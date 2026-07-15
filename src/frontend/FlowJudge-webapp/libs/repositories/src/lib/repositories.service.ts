import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL, PagedResult, PaginationQueryParams } from '@flow-judge-webapp/common';
import { GetRepositoriesResponseItem } from './repositories.model';

@Injectable({ providedIn: 'root' })
export class RepositoriesService {
  #httpClient = inject(HttpClient);
  #repositoriesPathSegment = 'api/repositories' as const;
  #baseUrl = inject(API_BASE_URL);

  getRepositoriesGridData(pageNumber: number, pageSize: number): Observable<PagedResult<GetRepositoriesResponseItem>> {
    const params = {
      pageNumber,
      pageSize,
    } satisfies PaginationQueryParams;
    const url = `${this.#baseUrl}/${this.#repositoriesPathSegment}`;

    return this.#httpClient.get<PagedResult<GetRepositoriesResponseItem>>(url, { params });
  }
}
