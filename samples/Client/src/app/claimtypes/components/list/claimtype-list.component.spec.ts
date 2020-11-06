import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClaimtypeListComponent } from './claimtype-list.component';

describe('ClaimtypeListComponent', () => {
  let component: ClaimtypeListComponent;
  let fixture: ComponentFixture<ClaimtypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClaimtypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClaimtypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
