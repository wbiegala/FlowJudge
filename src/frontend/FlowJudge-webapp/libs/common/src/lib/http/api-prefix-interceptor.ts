import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { API_BASE_URL } from './api-base-url.token';

const isAbsolute = (url: string) => /^[a-z][a-z0-9+.-]*:\/\//i.test(url);

export const apiPrefixInterceptor: HttpInterceptorFn = (req, next) => {
  const base = inject(API_BASE_URL, { optional: true }) ?? '';

  if (isAbsolute(req.url) || req.url.startsWith('/assets')) {
    return next(req);
  }

  if (!base) {
    return next(req);
  }

  if (req.url.startsWith('/api') || req.url.startsWith('/auth')) {
    const joined = `${base.replace(/\/+$/, '')}/${req.url.replace(/^\/+/, '')}`;
    return next(req.clone({ url: joined }));
  }

  return next(req);
};
