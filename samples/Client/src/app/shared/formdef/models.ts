import { Input, Directive } from '@angular/core';
import {
  ValidatorFn,
  Validators,
  AbstractControl
} from '@angular/forms';

export const HIDDEN_EDITOR = 'hidden';
export const TEXT_EDITOR = 'text';
export const TEXT_AREA_EDITOR = 'textarea';
export const PASSWORD_EDITOR = 'password';
export const RADIO_EDITOR = 'radio';
export const CHECKBOX_EDITOR = 'checkbox';
export const DATE_EDITOR = 'date';
export const NUMBER_EDITOR = 'number';
export const RANGE_EDITOR = 'range';
export const TIME_EDITOR = 'time';
export const SELECT_EDITOR = 'select';
export const MULTI_SELECT_EDITOR = 'multi-select';

export interface Editor {
  key: string;
  type: string;
  label: string;
  value?: any;
  isReadOnly?: boolean;
  options?: Array<{ key: string | number, value: string }>;
  singleSelection?: boolean;
  bindingBehaviour?: 'key' | 'value';
  required?: boolean;
  min?: number;
  max?: number;
  minLength?: number;
  maxLength?: number;
  allowAddingItems?: boolean;
}

export const SINGLE_SLOT = 'single';
export const ARRAY_SLOT = 'array';

export interface Slot {
  key: string;
  type: string;
  title: string;
  editors: Array<Editor>;
  children?: Array<Slot>;
}

export class FormdefValidator {
  public static getValidators(editor: Editor): ValidatorFn {
    const validators: Array<ValidatorFn> = new Array<ValidatorFn>();

    if (editor.required) {
      validators.push(Validators.required);
    }
    if (editor.min) {
      validators.push(Validators.min(editor.min));
    }
    if (editor.max) {
      validators.push(Validators.max(editor.max));
    }
    if (editor.minLength) {
      validators.push(Validators.minLength(editor.minLength));
    }
    if (editor.maxLength) {
      validators.push(Validators.maxLength(editor.maxLength));
    }

    return Validators.compose(validators);
  }
}

@Directive()
export class BaseSlotDirective {
  @Input()
  public slot: Slot;

  @Input()
  public form: AbstractControl;
}
