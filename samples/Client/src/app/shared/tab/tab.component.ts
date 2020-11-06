import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-tab',
  template: `<ng-content *ngIf="active"></ng-content>`
})
export class TabComponent {
  @HostBinding('class')
  public style = 'tabs__tab';

  @Input()
  public title: string;

  @Input()
  public active = false;
}
