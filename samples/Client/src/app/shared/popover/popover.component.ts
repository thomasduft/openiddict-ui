import { Component, OnInit, TemplateRef } from '@angular/core';

import { PopoverRef, PopoverContent } from './popover-ref';

@Component({
  template: `
  <div class="popover">
    <ng-container [ngSwitch]="renderMethod">
      <div *ngSwitchCase="'text'" [innerHTML]="content"></div>
      <ng-container *ngSwitchCase="'template'">
        <ng-container *ngTemplateOutlet="content; context: context"></ng-container>
      </ng-container>
      <ng-container *ngSwitchCase="'component'">
        <ng-container *ngComponentOutlet="content"></ng-container>
      </ng-container>
    </ng-container>
  </div>
  `
})
export class PopoverComponent implements OnInit {
  public renderMethod: 'template' | 'component' | 'text' = 'component';
  public content: PopoverContent;
  public context: any;

  public constructor(
    private popoverRef: PopoverRef
  ) { }

  public ngOnInit(): void {
    this.content = this.popoverRef.content;

    if (typeof this.content === 'string') {
      this.renderMethod = 'text';
    }

    if (this.content instanceof TemplateRef) {
      this.renderMethod = 'template';
      this.context = {
        close: this.popoverRef.close.bind(this.popoverRef)
      };
    }
  }
}
