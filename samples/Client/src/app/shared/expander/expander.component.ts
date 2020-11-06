import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-expander',
  templateUrl: './expander.component.html',
  styleUrls: ['./expander.component.less']
})
export class ExpanderComponent {
  @HostBinding('class')
  public style = 'expander';

  @Input()
  public open = false;
}
