import { Component, Input } from '@angular/core';

@Component({
  selector: 'tw-icon',
  template: `<i [ngClass]="classList" aria-hidden="true"></i>`
})
export class IconComponent {
  @Input()
  public set name(v: string) {
    // fa-'name'
    this.addClass(`fa-${v}`);
  }

  @Input()
  public set size(v: number) {
    // [1-5] fa-[lg|2-5]x
    this.addClass(`fa-${v}x`);
  }

  @Input()
  public set spin(v: boolean) {
    // true fa-spin
    v
      ? this.addClass('fa-spin')
      : this.removeClass('fa-spin');
  }

  public classList: Array<string> = new Array<string>(...['fa', 'fa-fw']);

  private addClass(v: string): void {
    this.classList.push(v);
  }

  private removeClass(v: string): void {
    let index: number;
    // tslint:disable-next-line: no-conditional-assignment
    if ((index = this.classList.indexOf(v)) >= 0) {
      this.classList.splice(index, 1);
    }
  }
}
