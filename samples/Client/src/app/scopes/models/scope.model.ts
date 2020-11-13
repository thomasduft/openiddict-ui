export interface Scope {
  id: string;
  enabled: boolean;
  name: string;
  displayName: string;
  description: string;
  required: boolean;
  emphasize: boolean;
  showInDiscoveryDocument: boolean;
  userClaims: Array<string>;
}
