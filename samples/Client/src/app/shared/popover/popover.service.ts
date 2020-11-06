import { Injectable, Injector } from '@angular/core';
import {
  Overlay,
  ConnectionPositionPair,
  PositionStrategy,
  OverlayConfig
} from '@angular/cdk/overlay';
import {
  PortalInjector,
  ComponentPortal
} from '@angular/cdk/portal';

import { PopoverRef, PopoverContent } from './popover-ref';
import { PopoverComponent } from './popover.component';

export interface PopoverParams<T> {
  width?: string | number;
  height?: string | number;
  hasBackdrop?: boolean;
  origin: HTMLElement;
  content: PopoverContent;
  data?: T;
}

@Injectable({
  providedIn: 'root'
})
export class Popover {
  public constructor(
    private overlay: Overlay,
    private injector: Injector
  ) { }

  public open<T>({
    origin,
    content,
    data,
    width,
    height,
    hasBackdrop = true
  }: PopoverParams<T>): PopoverRef<T> {
    const overlayRef = this.overlay.create(this.getOverlayConfig({
      origin,
      width,
      height,
      hasBackdrop
    }));
    const popoverRef = new PopoverRef<T>(overlayRef, content, data);

    const injector = this.createInjector(popoverRef, this.injector);
    overlayRef.attach(new ComponentPortal(PopoverComponent, null, injector));

    return popoverRef;
  }

  private createInjector(popoverRef: PopoverRef, injector: Injector): PortalInjector {
    const tokens = new WeakMap([[PopoverRef, popoverRef]]);

    return new PortalInjector(injector, tokens);
  }

  private getOverlayConfig({ origin, width, height, hasBackdrop }): OverlayConfig {
    return new OverlayConfig({
      hasBackdrop,
      width,
      height,
      backdropClass: 'popover-backdrop',
      positionStrategy: this.getOverlayPosition(origin),
      scrollStrategy: this.overlay.scrollStrategies.reposition()
    });
  }

  private getOverlayPosition(origin: HTMLElement): PositionStrategy {
    // centers the dialog in the viewport!
    const positionStrategy = this.overlay
      .position()
      .global()
      .centerHorizontally()
      .centerVertically();

    // const positionStrategy = this.overlay
    //   .position()
    //   .flexibleConnectedTo(origin)
    //   .withPositions(this.getPositions())
    //   .withFlexibleDimensions(false)
    //   .withPush(true);

    return positionStrategy;
  }

  private getPositions(): ConnectionPositionPair[] {
    return [
      {
        originX: 'center',
        originY: 'top',
        overlayX: 'center',
        overlayY: 'bottom'
      },
      {
        originX: 'center',
        originY: 'bottom',
        overlayX: 'center',
        overlayY: 'top',
      }
    ];
  }
}
