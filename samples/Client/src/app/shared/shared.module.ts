import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { IconModule } from './icon/icon.module';
import { ListModule } from './list/list.module';
import { PaneModule } from './pane/pane.module';
import { PopoverModule } from './popover/popover.module';

import { ForbiddenComponent } from './components/forbidden.component';
import { PageNotFoundComponent } from './components/page-not-found.component';

import { FilterPipe } from './pipes';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    IconModule,
    ListModule,
    PaneModule,
    PopoverModule
  ],
  declarations: [
    ForbiddenComponent,
    PageNotFoundComponent,
    FilterPipe
  ],
  exports: [
    ForbiddenComponent,
    PageNotFoundComponent,
    IconModule,
    ListModule,
    PaneModule,
    PopoverModule,
    FilterPipe
  ]
})
export class SharedModule { }
