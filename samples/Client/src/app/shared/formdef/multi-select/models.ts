export class ListItem {
  key: string | number;
  value: string;

  public constructor(source: any) {
    if (typeof source === 'string') {
      this.key = this.value = source;
    }
    if (typeof source === 'object') {
      this.key = source.key;
      this.value = source.value;
    }
  }

  public static byKey(key: string | number, items: Array<ListItem>): ListItem {
    return items.find(i => i.key === key);
  }

  public static byValue(value: string | number, items: Array<ListItem>): ListItem {
    return items.find(i => i.value === value);
  }
}
