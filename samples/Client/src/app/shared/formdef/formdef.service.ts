import { Injectable } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  FormArray
} from '@angular/forms';

import {
  FormdefValidator,
  Editor,
  Slot
} from './models';
import { FormdefRegistry } from './formdefRegistry.service';

@Injectable()
export class FormdefService {
  public constructor(
    private _fb: FormBuilder,
    private _slotRegistry: FormdefRegistry
  ) { }

  public toGroup(key: string, viewModel: any): FormGroup {
    const slot = this.getSlot(key);

    const fg = this.toGroupRecursive(slot, viewModel);

    return <FormGroup>fg;
  }

  public getSlot(key: string): Slot {
    return this._slotRegistry.get(key);
  }

  private toGroupRecursive(slot: Slot, viewModel: any): FormGroup | FormArray {
    const fg = this._fb.group({});

    const isArray = Array.isArray(viewModel);

    if (!isArray) {
      slot.editors.forEach((e: Editor) => {
        fg.addControl(e.key, new FormControl(
          { value: viewModel[e.key], disabled: e.isReadOnly === true },
          FormdefValidator.getValidators(e)
        ));
      });
    } else {
      const fa = this._fb.array([]);

      for (let i = 0; i < viewModel.length; i++) {
        const row = this._fb.group({});

        slot.editors.forEach((e: Editor) => {
          row.addControl(e.key, new FormControl(
            { value: viewModel[i][e.key], disabled: e.isReadOnly === true },
            FormdefValidator.getValidators(e)
          ));
        });

        fa.push(row);
      }

      return fa;
    }

    if (slot.children && slot.children.length > 0) {
      slot.children.forEach((child: Slot) => {
        fg.addControl(child.key, this.toGroupRecursive(child, viewModel[child.key]));
      });
    }

    return fg;
  }
}
