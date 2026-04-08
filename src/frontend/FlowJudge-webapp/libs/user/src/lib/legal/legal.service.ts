import { Observable } from 'rxjs';
import { API_BASE_URL } from '@flow-judge-webapp/common';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { GetDocumentVersionResponse } from './legal.model';

@Injectable({ providedIn: 'root' })
export class LegalService {
  #httpClient = inject(HttpClient);
  #legalPathSegment = 'api/legal' as const;
  #baseUrl = inject(API_BASE_URL);

  getActualTermsAndConditions(): Observable<GetDocumentVersionResponse> {
    const url = `${this.#baseUrl}/${this.#legalPathSegment}/terms-and-conditions`;

    return this.#httpClient.get<GetDocumentVersionResponse>(url);
  }

  getTermsAndConditions(versionId: string): Observable<GetDocumentVersionResponse> {
    //TODO: implement it!

    return this.getActualTermsAndConditions();
  }

  getActualPrivacyPolicy(): Observable<GetDocumentVersionResponse> {
    const url = `${this.#baseUrl}/${this.#legalPathSegment}/privacy-policy`;

    return this.#httpClient.get<GetDocumentVersionResponse>(url);
  }

    getPrivacyPolicy(versionId: string): Observable<GetDocumentVersionResponse> {
    //TODO: implement it!

    return this.getActualPrivacyPolicy();
  }
}
