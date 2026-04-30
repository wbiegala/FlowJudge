import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { API_BASE_URL, CreatedResponse, PagedResult, PaginationQueryParams } from '@flow-judge-webapp/common';
import { CreateWorkspaceRequest, GetWorkspaceResponse, GetWorkspacesResponseItem } from './workspaces.model';

@Injectable({ providedIn: 'root' })
export class WorkspacesService {
  #httpClient = inject(HttpClient);
  #workspacesPathSegment = 'api/workspaces' as const;
  #baseUrl = inject(API_BASE_URL);

  getWorkspacesGridData(pageNumber: number, pageSize: number): Observable<PagedResult<GetWorkspacesResponseItem>> {
    const params = {
      pageNumber: pageNumber,
      pageSize: pageSize,
    } satisfies PaginationQueryParams;

    const url = `${this.#baseUrl}/${this.#workspacesPathSegment}`;

    return this.#httpClient.get<PagedResult<GetWorkspacesResponseItem>>(url, { params: params })
  }

  getWorkspace(id: string): Observable<GetWorkspaceResponse> {
    const url = `${this.#baseUrl}/${this.#workspacesPathSegment}/${id}`;

    return this.#httpClient.get<GetWorkspaceResponse>(url);
  }

  createWorkspace(name: string): Observable<CreatedResponse> {
    const dto = {
      name: name,
    } satisfies CreateWorkspaceRequest;
    const url = `${this.#baseUrl}/${this.#workspacesPathSegment}`;

    return this.#httpClient.post<CreatedResponse>(url, dto);
  }
}
