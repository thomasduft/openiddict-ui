import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceDashboardComponent } from './resource-dashboard.component';

describe('ResourceDashboardComponent', () => {
  let component: ResourceDashboardComponent;
  let fixture: ComponentFixture<ResourceDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
