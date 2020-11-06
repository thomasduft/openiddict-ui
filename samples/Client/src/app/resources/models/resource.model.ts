export interface Resource {
  id: number;
  enabled: boolean;
  name: string;
  displayName: string;
  userClaims: Array<string>;
  scopes: Array<string>;
}
