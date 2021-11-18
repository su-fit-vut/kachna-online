import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountPopupComponent } from './account-popup.component';

describe('AccountPopupComponent', () => {
  let component: AccountPopupComponent;
  let fixture: ComponentFixture<AccountPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AccountPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
