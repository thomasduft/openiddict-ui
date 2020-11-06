import { Component, HostBinding, OnInit } from '@angular/core';

import { StatusMessage, StatusLevel } from '../../../shared/services';
import { StatusBarService } from './statusbar.service';

@Component({
  selector: 'tw-statusbar',
  providers: [
    StatusBarService
  ],
  template: `
  <div *ngIf="displayStatusBar()">
    <div *ngIf="hasAction">
      <button type="button"
              class="close"
              (click)="action()">
        <tw-icon name="bolt"></tw-icon>
      </button>
    </div>
    <div>
      <span class="workspace__status-close">
        <span (click)="close()">&times;</span>
      </span>
      {{ statusMessage }}
    </div>
  </div>`
})
export class StatusBarComponent implements OnInit {
  private message: StatusMessage;

  @HostBinding('class')
  public styles = this.getStyles();

  public hasAction = false;

  public get statusMessage(): string {
    return this.status && this.status.title && this.status.title.length > 0
      ? `${this.status.title}: ${this.status.message}`
      : this.status.message;
  }

  public get status(): StatusMessage {
    return this.message;
  }

  public constructor(
    private service: StatusBarService
  ) { }

  public ngOnInit(): void {
    this.service.status
      .subscribe((status: StatusMessage) => {
        if (!status) {
          // initial StatusMessage will be null!
          return;
        }

        this.message = status;
        this.hasAction = this.message.hasAction;
        this.fadeOut();

        this.styles = this.getStyles(this.getStatusClass());
      });
  }

  public displayStatusBar(): boolean {
    return this.message && !this.message.viewed;
  }

  public close(): void {
    this.message.viewed = true;
    this.styles = this.getStyles();
  }

  public action(): void {
    this.message.action();
    this.close();
  }

  private fadeOut(): void {
    if (!this.message
      || this.message.level !== StatusLevel.Success) {
      return;
    }

    setTimeout(() => {
      this.close();
    }, 5000);
  }

  private getStyles(status: string = null): string {
    if (status) {
      return `workspace__status ${status}`;
    }

    return 'workspace__status';
  }

  private getStatusClass(): string {
    let s = 'success';

    switch (this.message.level) {
      case StatusLevel.Info:
        s = 'info';
        break;
      case StatusLevel.Warning:
        s = 'warning';
        break;
      case StatusLevel.Danger:
        s = 'danger';
        break;
    }

    return `workspace__status--${s}`;
  }
}
