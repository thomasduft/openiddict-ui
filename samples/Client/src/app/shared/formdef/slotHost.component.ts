import {
  Component,
  OnInit,
  OnDestroy,
  Input,
  ComponentRef,
  ViewChild,
  ViewContainerRef,
  ComponentFactoryResolver
} from '@angular/core';
import {
  FormGroup
} from '@angular/forms';

import { Slot, SINGLE_SLOT, BaseSlotDirective } from './models';
import { SlotComponentRegistry } from './slotComponentRegistry.service';

@Component({
  selector: 'tw-slothost',
  template: `<ng-template #slotContent></ng-template>`
})
export class SlotHostComponent implements OnInit, OnDestroy {
  private slotContentRef: ComponentRef<BaseSlotDirective>;

  @Input()
  public slot: Slot;

  @Input()
  public form: FormGroup;

  @ViewChild('slotContent', { read: ViewContainerRef, static: true })
  protected slotContent: ViewContainerRef;

  public constructor(
    private registry: SlotComponentRegistry
  ) { }

  public ngOnInit(): void {
    if (this.slot) {
      const slotType = this.slot.type ? this.slot.type : SINGLE_SLOT;
      const context = this.registry.get(slotType);

      if (context) {
        const factory = this.slotContent.injector
          .get(ComponentFactoryResolver)
          .resolveComponentFactory(context.component);

        this.slotContentRef = this.slotContent
          .createComponent<BaseSlotDirective>(
            factory,
            this.slotContent.length
          );

        // @Input bindings
        this.slotContentRef.instance.slot = this.slot;
        this.slotContentRef.instance.form = this.form;
      }
    }
  }

  public ngOnDestroy(): void {
    if (this.slotContentRef) {
      delete this.slotContentRef.instance.slot;
      delete this.slotContentRef.instance.form;

      this.slotContentRef.destroy();
    }
  }
}
