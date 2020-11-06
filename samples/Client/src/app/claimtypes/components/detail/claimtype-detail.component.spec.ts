import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaimtypeDetailComponent } from './claimtype-detail.component';

describe('ClaimtypeDetailComponent', () => {
  let component: ClaimtypeDetailComponent;
  let fixture: ComponentFixture<ClaimtypeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClaimtypeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClaimtypeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
