import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from '../icon/icon.module';

import {
  ListComponent,
  FooterComponent,
  HeaderComponent,
  TwTemplateDirective
} from './list.component';

@NgModule({
  imports: [
    CommonModule,
    IconModule
  ],
  declarations: [
    ListComponent,
    HeaderComponent,
    FooterComponent,
    TwTemplateDirective
  ],
  exports: [
    ListComponent,
    HeaderComponent,
    FooterComponent,
    TwTemplateDirective
  ]
})
export class ListModule { }
