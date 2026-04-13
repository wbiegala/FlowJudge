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
import { apiPrefixInterceptor, provideApiBaseUrl, provideAppErrorHandler } from '@flow-judge-webapp/common';
import { App } from './app/app';
import { appRoutes } from './app/app.routes';
import { environment } from './environments/environment';
import { accessTokenInterceptor, provideRestoreSessionFactory, AuthenticationState } from '@flow-judge-webapp/auth';


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
          withInterceptors([apiPrefixInterceptor, accessTokenInterceptor]),
        ),
        provideApiBaseUrl(cfg.apiUrl),
        { provide: APP_CONFIG, useValue: cfg },
        provideAnimations(),
        provideBrowserGlobalErrorListeners(),
        provideZonelessChangeDetection(),
        provideStore([
          AuthenticationState
        ], withNgxsReduxDevtoolsPlugin(), withNgxsRouterPlugin(), withNgxsFormPlugin()),
        provideTranslateService({
          loader: { provide: TranslateLoader, useFactory: translationHttpLoaderFactory, deps: [HttpBackend] },
          lang: 'pl',
          fallbackLang: 'en'
        })
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
      { prefix: '/assets/i18n/ui/', suffix: '.json'}
    ]);
  }
