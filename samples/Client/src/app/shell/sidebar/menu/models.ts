export interface MenuItem {
  id: string;
  name: string;
  route?: string;
  icon?: string;
  children?: Array<MenuItem>;
}
