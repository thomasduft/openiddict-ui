export interface Client {
  id: number;
  enabled: boolean;
  clientId: string;
  clientName: string;
  requireClientSecret: boolean;
  clientSecret: string;
  requirePkce: boolean;
  requireConsent: boolean;
  allowAccessTokensViaBrowser: boolean;
  allowedGrantTypes: Array<string>;
  redirectUris: Array<string>;
  postLogoutRedirectUris: Array<string>;
  allowedCorsOrigins: Array<string>;
  allowedScopes: Array<string>;
}
