import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TabsComponent } from './tabs.component';
import { TabComponent } from './tab.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    TabsComponent,
    TabComponent
  ],
  exports: [
    TabsComponent,
    TabComponent
  ]
})
export class TabModule { }
