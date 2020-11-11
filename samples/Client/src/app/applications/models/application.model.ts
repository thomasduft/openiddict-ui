export interface Application {
  id: string;
  clientId: string;
  displayName: string;
  clientSecret: string;
  requirePkce: boolean;
  requireConsent: boolean;
  redirectUris: Array<string>;
  postLogoutRedirectUris: Array<string>;
  permissions: Array<string>;
}
