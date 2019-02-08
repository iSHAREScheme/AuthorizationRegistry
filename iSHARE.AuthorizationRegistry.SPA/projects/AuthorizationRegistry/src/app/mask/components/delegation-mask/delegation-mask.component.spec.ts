import { DelegationMaskComponent } from './delegation-mask.component';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

describe('CreatePolicyComponent', () => {
  let component: DelegationMaskComponent;
  let fixture: ComponentFixture<DelegationMaskComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DelegationMaskComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DelegationMaskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
