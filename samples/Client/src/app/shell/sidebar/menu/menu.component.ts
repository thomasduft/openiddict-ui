import { Component, OnInit } from '@angular/core';

import { MenuItem } from './models';
import { MenuService } from './menu.service';

@Component({
  selector: 'tw-menu',
  providers: [
    MenuService
  ],
  template: `
  <li [routerLinkActive]="'active'"
      [ngClass]="{'menu__item': item.route}"
      *ngFor="let item of menuItems">
    <a *ngIf="item.route" [routerLink]="item.route">
      <tw-icon *ngIf="item.icon" [name]="item.icon"></tw-icon>
      {{ item.name }}
    </a>
    <span *ngIf="!item.route">
      <tw-icon *ngIf="item.icon" [name]="item.icon"></tw-icon>
      {{ item.name }}
      <ul *ngIf="item.children && item.children.length > 0">
        <li routerLinkActive="active" class="menu__item" *ngFor="let child of item.children">
          <a *ngIf="child.route" [routerLink]="child.route">
            <tw-icon *ngIf="child.icon" [name]="child.icon"></tw-icon>
            {{ child.name }}
          </a>
        </li>
      </ul>
    </span>
  </li>
  `
})
export class MenuComponent implements OnInit {
  public get menuItems(): Array<MenuItem> {
    return this.service.items;
  }

  public constructor(
    private service: MenuService
  ) { }

  public ngOnInit(): void {
    this.service.register({
      id: '1',
      name: 'Users',
      route: '/users',
      icon: 'users'
    });

    this.service.register({
      id: '2',
      name: 'Roles',
      route: '/roles',
      icon: 'tags'
    });

    this.service.register({
      id: '3',
      name: 'ClaimTypes',
      route: '/claimtypes',
      icon: 'key'
    });

    this.service.register({
      id: '4',
      name: 'Scopes',
      route: '/scopes',
      icon: 'globe'
    });

    this.service.register({
      id: '5',
      name: 'Applications',
      route: '/applications',
      icon: 'desktop'
    });
  }
}
