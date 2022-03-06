import { RegisterApplication } from './register-application.model';
export interface Application extends RegisterApplication  {
  redirectUris: Array<string>;
  postLogoutRedirectUris: Array<string>;
  permissions: Array<string>;
}
