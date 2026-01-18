import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileService, EmailPreferencesDto } from '../../proxy/settings';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-email-preferences',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './email-preferences.html',
  styleUrl: './email-preferences.scss'
})
export class EmailPreferences implements OnInit {
  form: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadPreferences();
  }

  buildForm() {
    this.form = this.fb.group({
      newsletter: [true],
      linkSharing: [true],
      securityAlerts: [true],
      weeklyDigest: [true]
    });
  }

  loadPreferences() {
    this.isLoading = true;
    this.profileService.getEmailPreferences().subscribe({
      next: (res) => {
        this.form.patchValue(res);
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    this.isLoading = true;
    const input: EmailPreferencesDto = this.form.value;

    this.profileService.updateEmailPreferences(input).subscribe({
      next: () => {
        this.toaster.success('Email preferences updated.', 'Success');
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error('An error occurred.', 'Error');
        this.isLoading = false;
      }
    });
  }
}
