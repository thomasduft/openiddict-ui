import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { MultiSelectModule } from './multi-select/multi-select.module';

import { FormdefRegistry } from './formdefRegistry.service';

import { SINGLE_SLOT, ARRAY_SLOT } from './models';
import { ArraySlotComponent } from './arraySlot.component';
import { DateValueAccessorDirective } from './dateValueAccessor';
import { SlotComponent } from './slot.component';
import { EditorComponent } from './editor.component';
import { FormdefComponent } from './formdef.component';
import { SlotComponentRegistry, SlotComponentMetaData } from './slotComponentRegistry.service';
import { SlotHostComponent } from './slotHost.component';
import { ValidationSummaryComponent } from './validationsummary.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MultiSelectModule
  ],
  declarations: [
    EditorComponent,
    FormdefComponent,
    SlotHostComponent,
    SlotComponent,
    ArraySlotComponent,
    DateValueAccessorDirective,
    ValidationSummaryComponent
  ],
  exports: [
    ReactiveFormsModule,
    FormdefComponent,
    EditorComponent,
    ValidationSummaryComponent
  ],
  providers: [
    FormdefRegistry,
    SlotComponentRegistry
  ]
})
export class FormdefModule {
  public constructor(
    private registry: SlotComponentRegistry
  ) {
    this.registry.register(new SlotComponentMetaData(SINGLE_SLOT, SlotComponent));
    this.registry.register(new SlotComponentMetaData(ARRAY_SLOT, ArraySlotComponent));
  }
}
