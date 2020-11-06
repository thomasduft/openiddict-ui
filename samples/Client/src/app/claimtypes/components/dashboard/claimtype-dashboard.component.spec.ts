import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaimtypeDashboardComponent } from './claimtype-dashboard.component';

describe('ClaimtypeDashboardComponent', () => {
  let component: ClaimtypeDashboardComponent;
  let fixture: ComponentFixture<ClaimtypeDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClaimtypeDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClaimtypeDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
