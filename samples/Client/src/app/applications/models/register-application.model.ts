export interface RegisterApplication {
  id: string;
  clientId: string;
  displayName: string;
  type: string;
  clientSecret: string;
  requirePkce: boolean;
  requireConsent: boolean;
}