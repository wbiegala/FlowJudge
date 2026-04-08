import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { catchError, finalize, map, Observable, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { Store } from '@ngxs/store';
import { Navigate } from '@ngxs/router-plugin';
import { AuthenticationService } from './authentication.service';
import { AuthenticationState } from './store/authentication.state';

import { Authenticate, ClearAuthenticatedUserContext } from './store/authentication.actions';

// one shared "in-flight" refresh
// when non-null - all subscribers joins and waiting for result
let refreshInFlight$: Observable<string> | null = null;

const isAbsolute = (url: string) => /^[a-z][a-z0-9+.-]*:\/\//i.test(url);
const isRefreshEndpoint = (url: string) =>
  url.startsWith('api/auth/refresh-token') || url.includes('auth/refresh-token');

export const accessTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const authService = inject(AuthenticationService);

  // skipping Authorization and localurls
  const shouldAttachAuth =
    !isAbsolute(req.url) &&
    !req.url.startsWith('/assets') &&
    !isRefreshEndpoint(req.url);

  const currentToken = store.selectSnapshot(AuthenticationState.accessToken);

  const withAuth = (r: HttpRequest<unknown>, token: string | null) =>
    token ? r.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : r;

  const initialReq = shouldAttachAuth ? withAuth(req, currentToken) : req;

  return next(initialReq).pipe(
    catchError((err: HttpErrorResponse) => {
      // if not refresh and 401 -> try refresh
      if (err.status === 401 && !isRefreshEndpoint(req.url)) {
        // start refresh only one, rest will join to existing Observable
        if (!refreshInFlight$) {
          refreshInFlight$ = authService.refreshToken().pipe(
            tap(response =>
              store.dispatch(new Authenticate(response.accessToken, response.identityToken))
            ),
            // after state update - get new token
            map(() => store.selectSnapshot(AuthenticationState.accessToken) ?? ''),
            // share result to rest of subscribers
            shareReplay(1),
            // clear "in-flight" after success/fail
            finalize(() => {
              refreshInFlight$ = null;
            })
          );
        }

        return refreshInFlight$.pipe(
          switchMap((newToken) => {
            // call again orginal request
            const retried = shouldAttachAuth ? withAuth(req, newToken) : req;
            return next(retried);
          }),
          // if refresh fails - clear user context and redirect to /session-expired
          catchError(e => {
            store.dispatch([
              new ClearAuthenticatedUserContext(),
              new Navigate(['/session-expired'])
            ]);
            return throwError(() => e);
          })
        );
      }

      // other errors pass throw
      return throwError(() => err);
    })
  );
};
