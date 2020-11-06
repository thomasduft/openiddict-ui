import { User } from './user.model';

export interface UserDetail {
  user: User;
  claims: Array<string>;
  roles: Array<string>;
}
