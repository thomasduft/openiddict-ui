import { Injectable, Type } from '@angular/core';

export class SlotComponentMetaData {
  public constructor(public key: string, public component: Type<any>) { }
}

@Injectable()
export class SlotComponentRegistry {
  private _registry: Map<string, SlotComponentMetaData> = new Map<string, SlotComponentMetaData>();

  public register(cmp: SlotComponentMetaData): void {
    if (this._registry.has(cmp.key)) { return; }

    this._registry.set(cmp.key, cmp);
  }

  public exists(key: string): boolean {
    return this._registry.has(key);
  }

  public get(key: string): SlotComponentMetaData {
    const item = this._registry.get(key);

    if (!item) { throw new Error(`ComponentMetaData for key ${key} was not found!`); }

    return item;
  }
}
