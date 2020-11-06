export interface Claim {
  type: string;
  value: string;
}

export interface User {
  id: string;
  userName: string;
  email: string;
  lockoutEnabled: boolean;
  isLockedOut: boolean;
  claims: Array<Claim>;
  roles: Array<string>;
}
