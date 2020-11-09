import { Observable, throwError } from 'rxjs';

import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams,
  HttpErrorResponse,
  HttpEvent,
  HttpRequest
} from '@angular/common/http';

import { IdentityResult } from './models';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class HttpWrapperService {
  public constructor(
    private http: HttpClient,
    private api: ApiService
  ) { }

  public get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    const url = this.api.createApiUrl(endpoint);

    return this.http.get<T>(url, { params });
  }

  public getRaw<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(endpoint, { params });
  }

  public post<T>(endpoint: string, body: any): Observable<T> {
    const url = this.api.createApiUrl(endpoint);

    return this.http.post<T>(url, body);
  }

  public postFile(
    endpoint: string,
    file: File,
    useProgress: boolean = false
  ): Observable<HttpEvent<unknown>> {
    const url = this.api.createApiUrl(endpoint);
    const formData: FormData = new FormData();
    formData.append('file', file);

    const req = new HttpRequest(
      'POST',
      url,
      formData, {
      reportProgress: useProgress
    });

    return this.http.request(req);
  }

  public put<T>(endpoint: string, body: any): Observable<T> {
    const url = this.api.createApiUrl(endpoint);

    return this.http.put<T>(url, body);
  }

  public delete<T>(endpoint: string): Observable<T> {
    const url = this.api.createApiUrl(endpoint);

    return this.http.delete<T>(url);
  }

  public getBlob(endpoint: string): Observable<Blob> {
    const url = this.api.createApiUrl(endpoint);

    return this.http.get(url, { responseType: 'blob' });
  }

  public handleError(error: HttpErrorResponse) {
    console.log('Raw error', error);

    if (error.error instanceof ErrorEvent) {
      console.error('An error occured:', error);
    } else if (error.status === 400) {
      const errors = Array<string>();
      Object.keys(error.error).forEach(key => {
        if (Array.isArray(error.error[key])) {
          error.error[key].forEach(x => errors.push(x));
        }
      });

      const identityResult: IdentityResult = {
        succeeded: false,
        errors
      };
      return throwError(identityResult);
    } else {
      console.error(`Backend returned code ${error.status}: ${error.error.title}`);
    }

    return throwError('Somehting went wrong. Please try again later.');
  }
}
