import { Subject } from 'rxjs';

import { OverlayRef } from '@angular/cdk/overlay';
import { TemplateRef, Type } from '@angular/core';

export interface PopoverCloseEvent<T> {
  type: 'backdropClick' | 'close';
  data: T;
}

export type PopoverContent = TemplateRef<any> | Type<any> | string;

export class PopoverRef<T = any> {
  private afterClosed = new Subject<PopoverCloseEvent<T>>();

  public afterClosed$ = this.afterClosed.asObservable();

  public constructor(
    public overlay: OverlayRef,
    public content: PopoverContent,
    public data: T
  ) {
    overlay.backdropClick()
      .subscribe(() => {
        this.closeInternal('backdropClick', null);
      });
  }

  public close(data?: T) {
    this.closeInternal('close', data);
  }

  private closeInternal(type: 'backdropClick' | 'close', data?: T) {
    this.overlay.dispose();

    this.afterClosed.next({
      type,
      data
    });

    this.afterClosed.complete();
  }
}
