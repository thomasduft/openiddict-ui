import { Injectable, OnDestroy } from '@angular/core';

import { MenuItem } from './models';

@Injectable()
export class MenuService implements OnDestroy {
  public items: Array<MenuItem> = new Array<MenuItem>();

  public ngOnDestroy(): void {
    this.items = [];
  }

  public register(item: MenuItem): void {
    if (!this.items.some((m: MenuItem) => {
      return m.id === item.id;
    })) {
      this.items.push(item);
    }
  }
}
