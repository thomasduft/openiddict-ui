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
      .get<Array<Client>>('clients')
      .pipe(catchError(this.http.handleError));
  }

  public client(clientId: string): Observable<Client> {
    return this.http
      .get<Client>(`clients/${clientId}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: Client): Observable<string> {
    return this.http
      .post<string>('clients', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: Client): Observable<any> {
    return this.http
      .put<Client>('clients', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(clientId: string): Observable<any> {
    return this.http
      .delete<any>(`clients/${clientId}`)
      .pipe(catchError(this.http.handleError));
  }
}
