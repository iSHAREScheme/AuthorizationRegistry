import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyPolicyComponent } from './copy-policy.component';

describe('CopyPolicyComponent', () => {
  let component: CopyPolicyComponent;
  let fixture: ComponentFixture<CopyPolicyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CopyPolicyComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CopyPolicyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
