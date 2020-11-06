import { Injectable } from '@angular/core';

import { Slot } from './models';

@Injectable()
export class FormdefRegistry {
  private registry: Map<string, Slot>;

  public constructor() {
    this.registry = new Map<string, Slot>();
  }

  public register(slot: Slot) {
    this.registry.set(slot.key, slot);
  }

  public get(key: string): Slot {
    return this.registry.get(key);
  }
}
