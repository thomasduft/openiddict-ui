import {
  Component,
  ContentChildren,
  QueryList,
  AfterContentInit,
  HostBinding
} from '@angular/core';

import { TabComponent } from './tab.component';

@Component({
  selector: 'tw-tabs',
  template: `
  <ul>
    <li *ngFor="let tab of tabs"
      class="tabs__tab"
      [ngClass]="{ 'tab__title--active': tab.active }"
      [attr.tabindex]="getTabIndex(tab)"
      (click)="selectTab(tab)"
      (keydown)="keydownOnTab($event, tab)">
      <span class="tab__title">{{tab.title}}</span>
    </li>
  </ul>
  <ng-content></ng-content>`
})
export class TabsComponent implements AfterContentInit {
  @HostBinding('class')
  public style = 'tabs';

  @ContentChildren(TabComponent)
  public tabs: QueryList<TabComponent>;

  // contentChildren are set
  public ngAfterContentInit(): void {
    // get all active tabs
    const activeTabs = this.tabs.filter((tab) => tab.active);

    // if there is no active tab set, activate the first
    if (activeTabs.length === 0) {
      setTimeout(() => this.selectTab(this.tabs.first));
    }
  }

  public getTabIndex(tab: TabComponent): number | null {
    return !tab.active ? 0 : null;
  }

  public selectTab(tab: TabComponent) {
    // deactivate all tabs
    this.tabs.toArray().forEach(t => t.active = false);

    // activate the tab the user has clicked on.
    tab.active = true;
  }

  public keydownOnTab(event: KeyboardEvent, tab: TabComponent) {
    // enter and spacebar
    if (event.keyCode === 13 || event.keyCode === 32) {
      this.selectTab(tab);
    }
  }
}
