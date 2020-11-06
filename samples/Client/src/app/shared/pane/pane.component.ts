import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
  selector: 'tw-pane',
  templateUrl: './pane.component.html',
  styleUrls: ['./pane.component.less']
})
export class PaneComponent implements OnInit {
  @HostBinding('class')
  public style = 'pane';

  @Input()
  public onlydetail = false;

  public ngOnInit(): void {
    this.style = this.onlydetail ? 'pane pane--onlydetail' : 'pane';
  }
}
