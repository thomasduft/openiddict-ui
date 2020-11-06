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

import { Slot, SINGLE_SLOT } from './models';
import { SlotComponentRegistry } from './slotComponentRegistry.service';

@Component({
  selector: 'tw-slothost',
  template: `<ng-template #slotContent></ng-template>`
})
export class SlotHostComponent implements OnInit, OnDestroy {
  private _componentRef: ComponentRef<any>;
  private _slot: Slot;
  private _form: FormGroup;

  @Input()
  public set slot(slot: Slot) {
    this._slot = slot;
  }

  @Input()
  public set form(form: FormGroup) {
    this._form = form;
  }

  @ViewChild('slotContent', { read: ViewContainerRef, static: true })
  protected slotContent: ViewContainerRef;

  public constructor(
    private _registry: SlotComponentRegistry
  ) { }

  public ngOnInit(): void {
    if (this._slot) {
      const slotType = this._slot.type ? this._slot.type : SINGLE_SLOT;
      const context = this._registry.get(slotType);

      if (context) {
        const factory = this.slotContent.injector
          .get(ComponentFactoryResolver)
          .resolveComponentFactory(context.component);

        this._componentRef = this.slotContent.createComponent(
          factory,
          this.slotContent.length
        );

        // @Input bindings
        this._componentRef.instance.slot = this._slot;
        this._componentRef.instance.form = this._form;
      }
    }
  }

  public ngOnDestroy(): void {
    if (this._componentRef) {
      delete this._componentRef.instance.slot;
      delete this._componentRef.instance.form;

      this._componentRef.destroy();
    }
  }
}
