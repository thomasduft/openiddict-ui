import { Component } from '@angular/core';

import { BaseSlotDirective } from './models';

@Component({
  selector: 'tw-slot',
  template: `
  <ng-container *ngIf="slot && slot.editors && slot.editors.length > 0">
    <fieldset>
      <legend class="title" (click)="toggle()">{{ slot.title }}</legend>
      <ng-container *ngIf="!collapsed">
        <tw-editor
          *ngFor="let editor of slot.editors"
          [editor]="editor"
          [form]="form">
        </tw-editor>
      </ng-container>
    </fieldset>
  </ng-container>
  <ng-container *ngIf="slot && slot.children && slot.children.length > 0">
    <ng-container *ngFor="let child of slot.children">
      <tw-slothost
        [slot]="child"
        [form]="form.get(child.key)">
      </tw-slothost>
    </ng-container>
  </ng-container>`
})
export class SlotComponent extends BaseSlotDirective {
  public collapsed = false;

  public toggle(): void {
    this.collapsed = !this.collapsed;
  }
}
