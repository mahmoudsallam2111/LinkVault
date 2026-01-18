import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailPreferences } from './email-preferences';

describe('EmailPreferences', () => {
  let component: EmailPreferences;
  let fixture: ComponentFixture<EmailPreferences>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmailPreferences]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmailPreferences);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
