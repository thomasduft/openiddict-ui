import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../../shared/services/index';

import { Scope } from '../models/index';

@Injectable()
export class ScopeService {
  public constructor(
    private http: HttpWrapperService
  ) { }

  public scopes(): Observable<Array<Scope>> {
    return this.http
      .get<Array<Scope>>('scopes')
      .pipe(catchError(this.http.handleError));
  }

  public scopenames(): Observable<Array<string>> {
    return this.http
      .get<Array<string>>('scopes/names')
      .pipe(catchError(this.http.handleError));
  }

  public scope(name: string): Observable<Scope> {
    return this.http
      .get<Scope>(`scopes/${name}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: Scope): Observable<string> {
    model.id = undefined;
    return this.http
      .post<string>('scopes', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: Scope): Observable<any> {
    return this.http
      .put<Scope>('scopes', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(name: string): Observable<any> {
    return this.http
      .delete<any>(`scopes/${name}`)
      .pipe(catchError(this.http.handleError));
  }
}
