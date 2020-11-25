import {
  Component,
  forwardRef,
  Input,
  Output,
  EventEmitter,
  HostListener
} from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  ControlValueAccessor
} from '@angular/forms';

import { ListItem } from './models';

export const DROPDOWN_CONTROL_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => MultiSelectComponent),
  multi: true
};
const noop = () => { };

export const KEY_BINDING_BEHAVIOR = 'key';
export const VALUE_BINDING_BEHAVIOR = 'value';

@Component({
  selector: 'tw-multi-select',
  templateUrl: './multi-select.component.html',
  providers: [DROPDOWN_CONTROL_VALUE_ACCESSOR]
})
export class MultiSelectComponent implements ControlValueAccessor {
  public listItems: Array<ListItem> = [];

  public filter: ListItem = new ListItem(this.data);
  public selectedItems: Array<ListItem> = [];
  public isDropdownOpen = false;
  public disabled = false;

  public get sortedSelectedItems(): Array<ListItem> {
    return this.selectedItems.sort((a: ListItem, b: ListItem) => {
      if (a.key < b.key) { return -1; }
      if (a.key > b.key) { return 1; }
      return 0;
    });
  }

  public get sortedListItems(): Array<ListItem> {
    return this.listItems.sort((a: ListItem, b: ListItem) => {
      if (a.key < b.key) { return -1; }
      if (a.key > b.key) { return 1; }
      return 0;
    });
  }

  @Input()
  public singleSelection = false;

  @Input()
  public bindingBehavior: 'key' | 'value' = 'key';

  @Input()
  public allowAddingItems = false;

  @Input()
  public set data(value: Array<{ key: string | number, value: string }>) {
    if (!value) {
      this.listItems = [];
    } else {
      this.listItems = value
        .map((item: any) => new ListItem(item));
    }
  }

  @Output()
  public filterChange: EventEmitter<ListItem> = new EventEmitter<any>();

  @Output()
  public dropDownClose: EventEmitter<ListItem> = new EventEmitter<any>();

  @Output()
  public onSelect: EventEmitter<ListItem> = new EventEmitter<ListItem>();

  @Output()
  public selectAll: EventEmitter<Array<ListItem>> = new EventEmitter<Array<ListItem>>();

  @Output()
  public deSelect: EventEmitter<ListItem> = new EventEmitter<ListItem>();

  @Output()
  public deSelectAll: EventEmitter<Array<ListItem>> = new EventEmitter<Array<ListItem>>();

  private onTouchedCallback: () => void = noop;
  private onChangeCallback: (_: any) => void = noop;

  public onFilterTextChange($event) {
    this.filterChange.emit($event);
  }

  public onKeyDown(event: KeyboardEvent, item: ListItem): void {
    if (event.keyCode === 32) {
      // space bar
      event.preventDefault();

      this.onItemClick(item);
    }
  }

  public onItemClick(item: ListItem): void {
    if (this.disabled) {
      return;
    }

    const found = this.isSelected(item);
    if (!found) {
      this.addSelected(item);
    } else {
      this.removeSelected(item);
    }

    if (this.singleSelection) {
      this.closeDropdown();
    }
  }

  // From ControlValueAccessor interface
  public writeValue(value: any) {
    if (value === undefined) { return; }

    if (value !== undefined && value !== null && value.length > 0) {
      if (!Array.isArray(value)) {
        value = [value];
      }

      if (this.bindingBehavior === KEY_BINDING_BEHAVIOR) {
        this.selectedItems = value.map((item: any) => ListItem.byKey(item, this.listItems));
      } else {
        this.selectedItems = value.map((item: any) => ListItem.byValue(item, this.listItems));
      }
    } else {
      this.selectedItems = [];
    }

    this.onChangeCallback(value);
  }

  public registerOnChange(fn: any) {
    this.onChangeCallback = fn;
  }

  public registerOnTouched(fn: any) {
    this.onTouchedCallback = fn;
  }

  public setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  @HostListener('blur')
  public onTouched() {
    this.closeDropdown();
    this.onTouchedCallback();
  }

  @HostListener('window:keydown', ['$event'])
  public shortCuts(event: KeyboardEvent): void {
    if (event.keyCode === 13) {
      // return
      event.preventDefault();

      this.toggleDropdown(event);
    }
  }

  // tslint:disable-next-line:variable-name
  public trackByFn(_index, item: ListItem): any {
    return item.key;
  }

  public isSelected(clickedItem: ListItem): boolean {
    let found = false;
    this.selectedItems.forEach(item => {
      if (clickedItem.key === item.key) {
        found = true;
      }
    });

    return found;
  }

  public isAllItemsSelected(): boolean {
    return this.listItems.length === this.selectedItems.length;
  }

  public showButton(): boolean {
    if (!this.singleSelection) {
      return true;
    } else {
      return false;
    }
  }

  public addItem(value: string): void {
    const item = new ListItem(value);

    this.listItems.push(item);
  }

  public addSelected(item: ListItem): void {
    if (this.singleSelection) {
      this.selectedItems = [];
      this.selectedItems.push(item);
    } else {
      this.selectedItems.push(item);
    }

    this.onChangeCallback(this.emittedValue(this.selectedItems));
    this.onSelect.emit(this.emittedValue(item));
  }

  public removeSelected(itemSel: ListItem): void {
    this.selectedItems.forEach(item => {
      if (itemSel.key === item.key) {
        this.selectedItems.splice(this.selectedItems.indexOf(item), 1);
      }
    });

    this.onChangeCallback(this.emittedValue(this.selectedItems));
    this.deSelect.emit(this.emittedValue(itemSel));
  }

  public emittedValue(val: any): any {
    if (this.singleSelection) {
      // if singleSelection we treat it like a dropdown
      if (Array.isArray(val) && val.length > 0) {
        if (this.bindingBehavior === KEY_BINDING_BEHAVIOR) {
          return val[0].key;
        } else {
          return val[0].value;
        }
      } else if (!Array.isArray(val)) {
        if (this.bindingBehavior === KEY_BINDING_BEHAVIOR) {
          return val.key;
        } else {
          return val.value;
        }
      }

      return null;
    }

    const selected = [];
    if (Array.isArray(val)) {
      val.map((item: ListItem) => {
        if (this.bindingBehavior === KEY_BINDING_BEHAVIOR) {
          selected.push(item.key);
        } else {
          selected.push(item.value);
        }
      });
    } else {
      if (val) {
        if (this.bindingBehavior === KEY_BINDING_BEHAVIOR) {
          selected.push(val.key);
        } else {
          selected.push(val.value);
        }
      }
    }

    return selected;
  }

  public toggleDropdown(evt) {
    evt.preventDefault();

    if (this.disabled && this.singleSelection) {
      return;
    }

    this.isDropdownOpen = !this.isDropdownOpen;

    if (!this.isDropdownOpen) {
      this.dropDownClose.emit();
    }
  }

  public closeDropdown(): void {
    this.isDropdownOpen = false;
    // clear search text
    this.filter.value = '';

    this.dropDownClose.emit();
  }

  public toggleSelectAll(): boolean {
    if (this.disabled) {
      return false;
    }

    if (!this.isAllItemsSelected()) {
      this.selectedItems = this.listItems.slice();
      this.selectAll.emit(this.emittedValue(this.selectedItems));
    } else {
      this.selectedItems = [];
      this.deSelectAll.emit(this.emittedValue(this.selectedItems));
    }

    this.onChangeCallback(this.emittedValue(this.selectedItems));
  }
}
