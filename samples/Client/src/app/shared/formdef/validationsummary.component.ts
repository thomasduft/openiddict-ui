import { Component, Input } from '@angular/core';

@Component({
  selector: 'tw-validation-summary',
  template: `
  <ul class="form__validation-summary" *ngIf="errors">
    <li *ngFor="let error of errors">{{error}}</li>
  </ul>`
})
export class ValidationSummaryComponent {
  @Input()
  public errors: Array<string>;
}
