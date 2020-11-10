import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../../shared/services/index';

import { Client } from '../models/index';

@Injectable()
export class ClientService {
  public constructor(
    private http: HttpWrapperService
  ) { }

  public clients(): Observable<Array<Client>> {
    return this.http
      .get<Array<Client>>('application')
      .pipe(catchError(this.http.handleError));
  }

  public client(id: string): Observable<Client> {
    return this.http
      .get<Client>(`application/${id}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: Client): Observable<string> {
    model.id = undefined;
    return this.http
      .post<string>('application', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: Client): Observable<any> {
    return this.http
      .put<Client>('application', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(id: string): Observable<any> {
    return this.http
      .delete<any>(`application/${id}`)
      .pipe(catchError(this.http.handleError));
  }
}
