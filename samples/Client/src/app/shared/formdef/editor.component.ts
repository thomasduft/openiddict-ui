import {
  Component,
  Input,
  HostBinding
} from '@angular/core';
import {
  FormGroup,
  AbstractControl
} from '@angular/forms';

import { Editor } from './models';

@Component({
  selector: 'tw-editor',
  template: `
  <div [ngSwitch]="editor.type"
       [formGroup]="form"
       [ngClass]="{ 'form-group': editor.type !== 'checkbox',
                    'checkbox': editor.type === 'checkbox' }">
    <label *ngIf="(!hideLabel && editor.type !== 'checkbox' && editor.type !== 'hidden')"
           [attr.for]="editor.key">
      {{ editor.label }}
    </label>

   <input *ngSwitchCase="'hidden'"
          type="hidden"
          [attr.id]="editor.key"
          [formControlName]="editor.key" />

   <input *ngSwitchCase="'number'"
          type="number"
          pattern="[0-9.,]*"
          inputmode="numeric"
          [attr.id]="editor.key"
          [formControlName]="editor.key" />

   <input *ngSwitchCase="'password'"
          type="password"
          [attr.id]="editor.key"
          [formControlName]="editor.key" />

   <label *ngSwitchCase="'checkbox'">
     <input type="checkbox"
            [attr.id]="editor.key"
            [formControlName]="editor.key">
     {{ editor.label }}
   </label>

   <select *ngSwitchCase="'select'"
           [formControlName]="editor.key">
     <option *ngIf="!editor.required" [value]=""></option>
     <option *ngFor="let opt of editor.options" [ngValue]="opt.key">
       {{ opt.value }}
     </option>
   </select>

   <tw-multi-select *ngSwitchCase="'multi-select'"
                    [formControlName]="editor.key"
                    [singleSelection]="editor.singleSelection"
                    [bindingBehavior]="editor.bindingBehaviour"
                    [allowAddingItems]="editor.allowAddingItems"
                    [data]="editor.options">
   </tw-multi-select>

   <input *ngSwitchCase="'date'"
          type="date"
          [attr.id]="editor.key"
          [formControlName]="editor.key"
          twUseValueAsDate />

    <textarea *ngSwitchCase="'textarea'"
              [attr.id]="editor.key"
              [formControlName]="editor.key">
    </textarea>

   <input *ngSwitchDefault
          type="text"
          [attr.id]="editor.key"
          [formControlName]="editor.key" />

    <div *ngIf="control(editor.key).invalid" class="form__validation--error">
      <div *ngIf="control(editor.key).hasError('required')" i18n>
        {{ editor.label }} required.
      </div>
      <div *ngIf="control(editor.key).hasError('min')" i18n>
        {{ editor.label }} must not be lower than {{ editor.min }}.
      </div>
      <div *ngIf="control(editor.key).hasError('max')" i18n>
        {{ editor.label }} must not be greater than {{ editor.max }}.
      </div>
      <div *ngIf="control(editor.key).hasError('minlength')" i18n>
        {{ editor.label }} must be at least {{ editor.minLength }} characters in length.
      </div>
      <div *ngIf="control(editor.key).hasError('maxlength')" i18n>
        {{ editor.label }} must not be longer than {{ editor.maxLength }} characters.
      </div>
    </div>
  </div>`
})
export class EditorComponent {
  @HostBinding('class')
  public style = 'editor';

  @Input()
  public hideLabel = false;

  @Input()
  public editor: Editor;

  @Input()
  public form: FormGroup;

  public control(name: string): AbstractControl {
    return this.form.get(name);
  }
}
