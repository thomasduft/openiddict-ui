import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../../shared/services/index';

import {
  Role
} from '../models/index';

@Injectable()
export class RoleService {
  public constructor(
    private http: HttpWrapperService
  ) { }

  public roles(): Observable<Array<Role>> {
    return this.http
      .get<Array<Role>>('roles')
      .pipe(catchError(this.http.handleError));
  }

  public role(id: string): Observable<Role> {
    return this.http
      .get<Role>(`roles/${id}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: Role): Observable<string> {
    model.id = undefined;
    return this.http
      .post<string>('roles', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: Role): Observable<any> {
    return this.http
      .put<Role>('roles', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(id: string): Observable<any> {
    return this.http
      .delete<any>(`roles/${id}`)
      .pipe(catchError(this.http.handleError));
  }
}
