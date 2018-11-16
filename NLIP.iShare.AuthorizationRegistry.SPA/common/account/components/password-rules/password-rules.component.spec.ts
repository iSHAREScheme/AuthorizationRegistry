import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PasswordRulesComponent } from './password-rules.component';

describe('ChangePasswordComponent', () => {
  let component: PasswordRulesComponent;
  let fixture: ComponentFixture<PasswordRulesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PasswordRulesComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PasswordRulesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
