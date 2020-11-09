import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Router, CanActivate, CanActivateChild } from '@angular/router';
import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpErrorResponse,
  HttpResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';

import { OAuthService } from 'angular-oauth2-oidc';

import { StatusMessage, StatusLevel } from './models';
import { MessageBus } from './messageBus.service';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {
  public constructor(
    private oauthService: OAuthService
  ) { }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.oauthService.hasValidAccessToken()) {
      const token = this.oauthService.getAccessToken();
      req = req.clone({ headers: req.headers.set('Authorization', 'Bearer ' + token) });
    }

    return next.handle(req);
  }
}

@Injectable({
  providedIn: 'root'
})
export class HttpErrorInterceptor implements HttpInterceptor {
  public constructor(
    private router: Router,
    private messageBus: MessageBus
  ) { }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req)
      .pipe(tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          // do stuff with response if you want
        }
      }, (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 403) {
            this.router.navigate(['forbidden']);
          } else {
            console.log(err);
            this.messageBus.publish(new StatusMessage(
              err.statusText,
              err.message,
              StatusLevel.Danger
            ));
          }
        }
      }));
  }
}

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {
  public constructor(
    private oauthService: OAuthService,
    private router: Router
  ) { }

  public canActivate(): boolean {
    const can = this.oauthService.hasValidAccessToken();

    if (!can) {
      this.router.navigate(['forbidden']);
      // this.oauthService.initLoginFlow();
    }

    return can;
  }

  public canActivateChild(): boolean {
    return this.canActivate();
  }
}

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true }
];
