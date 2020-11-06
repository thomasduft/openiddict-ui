import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../../shared/services/index';

import { Resource } from '../models/index';

@Injectable()
export class ResourceService {
  public constructor(
    private http: HttpWrapperService
  ) { }

  public resources(): Observable<Array<Resource>> {
    return this.http
      .get<Array<Resource>>('resources')
      .pipe(catchError(this.http.handleError));
  }

  public resource(name: string): Observable<Resource> {
    return this.http
      .get<Resource>(`resources/${name}`)
      .pipe(catchError(this.http.handleError));
  }

  public create(model: Resource): Observable<string> {
    return this.http
      .post<string>('resources', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: Resource): Observable<any> {
    return this.http
      .put<Resource>('resources', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(name: string): Observable<any> {
    return this.http
      .delete<any>(`resources/${name}`)
      .pipe(catchError(this.http.handleError));
  }
}
