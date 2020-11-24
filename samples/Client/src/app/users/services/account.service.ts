import { Observable, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { Injectable } from '@angular/core';

import { HttpWrapperService, IdentityResult } from '../../shared/services/index';

import { ClaimTypesService } from '../../claimtypes/services';
import { RoleService } from '../../roles/services';

import {
  ChangePassword,
  RegisterUser,
  User,
  UserDetail
} from '../models/index';

@Injectable()
export class AccountService {
  public constructor(
    private http: HttpWrapperService,
    private claimsService: ClaimTypesService,
    private roleService: RoleService
  ) { }

  public users(): Observable<Array<User>> {
    return this.http
      .get<Array<User>>('accounts/users')
      .pipe(catchError(this.http.handleError));
  }

  public user(id: string): Observable<UserDetail> {
    return forkJoin({
      user: this.http.get<User>(`accounts/user/${id}`),
      claims: this.claimsService.claimtypes(),
      roles: this.roleService.roles()
    })
      .pipe(map(info => {
        return {
          user: info.user,
          claims: info.claims.map(c => c.name),
          roles: info.roles.map(r => r.name)
        };
      }))
      .pipe(catchError(this.http.handleError));
  }

  public register(model: RegisterUser): Observable<IdentityResult> {
    return this.http
      .post<IdentityResult>('accounts/register', model)
      .pipe(catchError(this.http.handleError));
  }

  public update(model: User): Observable<IdentityResult> {
    return this.http
      .put<IdentityResult>('accounts/user', model)
      .pipe(catchError(this.http.handleError));
  }

  public delete(id: string): Observable<any> {
    return this.http
      .delete<any>(`accounts/user/${id}`)
      .pipe(catchError(this.http.handleError));
  }
}
