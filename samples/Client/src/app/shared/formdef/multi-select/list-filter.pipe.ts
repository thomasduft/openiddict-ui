import { Pipe, PipeTransform } from '@angular/core';

import { ListItem } from './models';

@Pipe({
  name: 'tw2ListFilter',
  pure: false
})
export class ListFilterPipe implements PipeTransform {
  public transform(items: ListItem[], filter: ListItem): ListItem[] {
    if (!items || !filter) {
      return items;
    }
    return items.filter((item: ListItem) => this.applyFilter(item, filter));
  }

  public applyFilter(item: ListItem, filter: ListItem): boolean {
    return !(filter.value
      && item.value
      && item.value.toLowerCase().indexOf(filter.value.toLowerCase()) === -1);
  }
}
