import { API_BASE_URL } from '@flow-judge-webapp/common';
import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { GetUserLegalStateResponse, UserLegalMissings } from './user.model';

@Injectable({ providedIn: 'root' })
export class UserLegalService {
  #httpClient = inject(HttpClient);
  #userPathSegment = 'api/user' as const;
  #baseUrl = inject(API_BASE_URL);
  #isDefined = signal(false);
  readonly legalComponentUrl = '/legal-check';
  missings = signal<Array<UserLegalMissings>>([]);
  isLegal = computed(() => {
    if (this.#isDefined()) {
      return (this.missings()?.length ?? 0) === 0;
    } else {
      return null;
    }
  });

  acceptTermsAndConditions(versionId: string): Observable<unknown> {
    const url = `${this.#baseUrl}/${this.#userPathSegment}/terms-and-conditions/${versionId}/accept`;

    return this.#httpClient.put(url, null).pipe(
      tap(_ => this.missings.update(miss => miss = miss.filter(m => m !== 'TermsAndConditionsActualVersionAccepted'))),
    );
  }

  acceptPrivacyPolicy(versionId: string): Observable<unknown> {
    const url = `${this.#baseUrl}/${this.#userPathSegment}/privacy-policy/${versionId}/accept`;

    return this.#httpClient.put(url, null).pipe(
      tap(_ => this.missings.update(miss => miss = miss.filter(m => m !== 'PrivacyPolicyActualVersionAccepted'))),
    );
  }

  getUserLegalState(): Observable<GetUserLegalStateResponse> {
    const url = `${this.#baseUrl}/${this.#userPathSegment}/legal-state`;

    return this.#httpClient.get<GetUserLegalStateResponse>(url).pipe(
      tap(response => {
        this.#isDefined.set(true);
        this.missings.set(response.missings);
      })
    );
  }

  getUserAss() {
    const url = `${this.#baseUrl}/${this.#userPathSegment}/ass`;

    return this.#httpClient.get<unknown>(url);
  }
}
