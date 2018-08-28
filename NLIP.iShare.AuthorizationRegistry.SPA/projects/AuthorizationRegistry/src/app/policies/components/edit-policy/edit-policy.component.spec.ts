import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPolicyComponent } from './edit-policy.component';

describe('EditPolicyComponent', () => {
  let component: EditPolicyComponent;
  let fixture: ComponentFixture<EditPolicyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [EditPolicyComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditPolicyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
