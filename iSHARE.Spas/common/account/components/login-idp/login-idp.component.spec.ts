/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LoginIdpComponent } from './login-idp.component';

describe('LoginIdpComponent', () => {
  let component: LoginIdpComponent;
  let fixture: ComponentFixture<LoginIdpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoginIdpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginIdpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
