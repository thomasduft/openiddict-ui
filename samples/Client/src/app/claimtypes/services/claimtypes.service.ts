import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../../shared/services/index';

import {
  ClaimType
} from '../models/index';

@Injectable()
export class ClaimTypesService {
  public constructor(
    private http: HttpWrapperService
  ) { }

  public claimtypes(): Observable<Array<ClaimType>> {
    return this.http
      .get<Array<ClaimType>>('claimtypes')
      .pipe(catchError(this.http.handleError));
  }

  public claimtype(id: string): Observable<ClaimType> {
    return this.http
      .get<ClaimType>(`claimtypes/${id}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: ClaimType): Observable<string> {
    model.id = undefined;
    return this.http
      .post<string>('claimtypes', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: ClaimType): Observable<any> {
    return this.http
      .put<ClaimType>('claimtypes', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(id: string): Observable<any> {
    return this.http
      .delete<any>(`claimtypes/${id}`)
      .pipe(catchError(this.http.handleError));
  }
}
