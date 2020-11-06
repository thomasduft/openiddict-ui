import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScopeDetailComponent } from './scope-detail.component';

describe('ScopeDetailComponent', () => {
  let component: ScopeDetailComponent;
  let fixture: ComponentFixture<ScopeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ScopeDetailComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScopeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
