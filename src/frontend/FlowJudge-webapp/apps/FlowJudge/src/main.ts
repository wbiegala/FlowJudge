import { HttpBackend, provideHttpClient, withInterceptors } from '@angular/common/http';
import { bootstrapApplication } from '@angular/platform-browser';
import { InjectionToken, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withRouterConfig } from '@angular/router';
import { TranslateLoader, provideTranslateService } from '@ngx-translate/core';
import { MultiTranslateHttpLoader } from 'ngx-translate-multi-http-loader';
import { provideStore } from '@ngxs/store';
import { withNgxsReduxDevtoolsPlugin } from '@ngxs/devtools-plugin';
import { withNgxsRouterPlugin } from '@ngxs/router-plugin';
import { withNgxsFormPlugin } from '@ngxs/form-plugin';
import { provideAnimations } from '@angular/platform-browser/animations';
import { apiPrefixInterceptor, appErrorInterceptor, provideApiBaseUrl, provideAppErrorHandler } from '@flow-judge-webapp/common';
import { App } from './app/app';
import { appRoutes } from './app/app.routes';
import { environment } from './environments/environment';
import { accessTokenInterceptor, provideRestoreSessionFactory, AuthenticationState, InsufficientPermissionsErrorHandler, UnauthorizedErrorHandler } from '@flow-judge-webapp/auth';
import { DefaultHttpErrorHandler } from './app/utils/default-http-error-handler';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { provideValidationErrors } from '@flow-judge-webapp/ui';
import { LegalErrorHandler } from './app/utils/legal-error-handler';
import { workspaceContextInterceptor, WorkspaceContextState } from '@flow-judge-webapp/workspaces';


fetch(environment.configUrl, { cache: 'no-store' })
  .then(r => {
    if (!r.ok) throw new Error(`Failed to load configuration.json (${r.status})`);
    return r.json();
  })
  .then((cfg: AppConfig) =>
    bootstrapApplication(App, {
      providers: [
        provideRouter(appRoutes, withRouterConfig({
          onSameUrlNavigation: 'reload',
        })),
        provideRestoreSessionFactory,
        provideHttpClient(
          withInterceptors([apiPrefixInterceptor, accessTokenInterceptor, workspaceContextInterceptor, appErrorInterceptor]),
        ),
        provideApiBaseUrl(cfg.apiUrl),
        { provide: APP_CONFIG, useValue: cfg },
        ...provideAppErrorHandling(),
        ...provideFormValidationErrorMappers(),
        provideAnimations(),
        provideBrowserGlobalErrorListeners(),
        provideZonelessChangeDetection(),
        provideStore([
          AuthenticationState,
          WorkspaceContextState,
        ],
          ...(!environment.production ? [withNgxsReduxDevtoolsPlugin()] : []),
          withNgxsRouterPlugin(),
          withNgxsFormPlugin(),
        ),
        provideTranslateService({
          loader: { provide: TranslateLoader, useFactory: translationHttpLoaderFactory, deps: [HttpBackend] },
          lang: 'pl',
          fallbackLang: 'en'
        }),
        {
          provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
          useValue: {
            appearance: 'outline',
            subscriptSizing: 'dynamic',
          },
        }
      ],
    })
  )

  export interface AppConfig {
    apiUrl: string;
  }

  export const APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG');

  export function translationHttpLoaderFactory(http: HttpBackend): TranslateLoader {
    return new MultiTranslateHttpLoader(http, [
      { prefix: '/assets/i18n/', suffix: '.json' },
      { prefix: '/assets/i18n/auth/', suffix: '.json' },
      { prefix: '/assets/i18n/user/', suffix: '.json'},
      { prefix: '/assets/i18n/ui/', suffix: '.json'},
      { prefix: '/assets/i18n/workspaces/', suffix: '.json' },
      { prefix: '/assets/i18n/integrations/', suffix: '.json' }
    ]);
  }

  export function provideAppErrorHandling(): any[] {
    return [
      provideAppErrorHandler(DefaultHttpErrorHandler),
      provideAppErrorHandler(InsufficientPermissionsErrorHandler),
      provideAppErrorHandler(LegalErrorHandler),
      provideAppErrorHandler(UnauthorizedErrorHandler)
    ];
  }

  export function provideFormValidationErrorMappers(): any[] {
    return [
      provideValidationErrors()
    ];
  }
