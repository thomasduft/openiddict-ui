import { Component } from '@angular/core';
import {
  UntypedFormGroup,
  UntypedFormArray,
  UntypedFormControl,
  UntypedFormBuilder
} from '@angular/forms';

import { FormdefValidator, Editor, Slot } from './models';
import { SlotComponent } from './slot.component';

@Component({
  selector: 'tw-arrayslot',
  template: `
  <fieldset>
    <legend class="title" (click)="toggle()">{{ slot.title }}</legend>
    <ng-container *ngIf="!collapsed">
      <div class="button__bar">
        <button type="button"
                class="button--secondary"
                title="add"
                i18n-title
                (click)="add()">
                <strong>+</strong>
        </button>
      </div>
      <div>
        <table [formGroup]="form">
          <tbody>
            <tr *ngFor="let row of rows.controls; let idx = index">
              <td *ngFor="let editor of slot.editors">
                <tw-editor
                  [hideLabel]="true"
                  [editor]="editor"
                  [form]="rows.at(idx)">
                </tw-editor>
              </td>
              <td>
                <button type="button"
                        class="button--secondary button--squared"
                        title="remove"
                        i18n-title
                        (click)="remove(idx)">
                        <strong>-</strong>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </ng-container>
  </fieldset>`
})
export class ArraySlotComponent extends SlotComponent {
  public get rows(): UntypedFormArray {
    return this.form as UntypedFormArray;
  }

  public constructor(
    private _fb: UntypedFormBuilder
  ) {
    super();
  }

  public add(): void {
    const row = this.createRow(this.slot);
    this.rows.push(row);
  }

  public remove(idx: number): void {
    this.rows.removeAt(idx);
  }

  protected createRow(arraySlot: Slot): UntypedFormGroup {
    const row = this._fb.group({});

    arraySlot.editors.forEach((e: Editor) => {
      row.addControl(e.key, new UntypedFormControl(undefined, FormdefValidator.getValidators(e)));
    });

    return row;
  }
}
