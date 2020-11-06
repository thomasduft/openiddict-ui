import { Directive, ElementRef, HostListener, Renderer2, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export const DATE_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => DateValueAccessorDirective),
  multi: true
};

/**
 * The accessor for writing a value and listening to changes on a date input element
 *
 *  ### Example
 *  `<input type="date" name="myBirthday" ngModel useValueAsDate>`
 *
 * see: https://blog.johanneshoppe.de/2016/10/angular-2-how-to-use-date-input-controls-with-angular-forms/
 */
@Directive({
  selector: '[twUseValueAsDate]',
  providers: [DATE_VALUE_ACCESSOR]
})
export class DateValueAccessorDirective implements ControlValueAccessor {
  @HostListener('input', ['$event.target.valueAsDate'])
  public onChange = (_: any) => { }

  @HostListener('blur', [])
  public onTouched = () => { }

  public constructor(
    private _renderer: Renderer2,
    private _elementRef: ElementRef
  ) { }

  public writeValue(value: Date): void {
    const v = new Date(value);
    this._renderer.setProperty(this._elementRef.nativeElement, 'valueAsDate', v);
  }

  public registerOnChange(fn: (_: any) => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  public setDisabledState(isDisabled: boolean): void {
    this._renderer.setProperty(this._elementRef.nativeElement, 'disabled', isDisabled);
  }
}
