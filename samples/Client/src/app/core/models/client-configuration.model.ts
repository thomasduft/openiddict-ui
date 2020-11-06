export interface ClientConfiguration {
  clientId: string;
  issuer: string;
  redirectUri: string;
  responseType: string;
  scope: string;
  loginUrl: string;
  logoutUrl: string;
}
