import { Component } from '@angular/core';

import { PopoverRef } from './popover-ref';

export interface DeleteConfirmation {
  itemText: string;
  confirm: boolean;
}

@Component({
  selector: 'tw-delete-confirmation',
  template: `
  <h3 i18n>Do you really want to delete '{{ deleteConfirmation.itemText }}'?</h3>
  <div class="button__bar button__bar--aligned-right">
    <button
      type="button"
      class="button--secondary"
      (click)="no()"
      i18n>No</button>
    <button
      type="button"
      (click)="yes()"
      i18n >Yes</button>
  </div>
  `
})
export class DeleteConfirmationComponent {
  public deleteConfirmation: DeleteConfirmation = {
    itemText: 'the item',
    confirm: false
  };

  public constructor(
    private popoverRef: PopoverRef
  ) {
    this.deleteConfirmation = this.popoverRef.data;
  }

  public no() {
    this.emit(false);
  }

  public yes() {
    this.emit(true);
  }

  private emit(confirm: boolean) {
    this.deleteConfirmation.confirm = confirm;
    this.popoverRef.close(this.deleteConfirmation);
  }
}
