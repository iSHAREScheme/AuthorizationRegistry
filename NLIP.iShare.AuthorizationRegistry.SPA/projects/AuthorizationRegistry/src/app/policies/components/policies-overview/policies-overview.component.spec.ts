import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PoliciesOverviewComponent } from './policies-overview.component';

describe('PoliciesOverviewComponent', () => {
  let component: PoliciesOverviewComponent;
  let fixture: ComponentFixture<PoliciesOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PoliciesOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PoliciesOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
