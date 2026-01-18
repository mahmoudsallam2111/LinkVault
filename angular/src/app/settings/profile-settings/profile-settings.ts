import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileService, UpdateProfileDto } from '../../proxy/settings';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-profile-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './profile-settings.html',
  styleUrl: './profile-settings.scss'
})
export class ProfileSettings implements OnInit {
  form: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadProfile();
  }

  buildForm() {
    this.form = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      name: ['', [Validators.maxLength(64)]],
      surname: ['', [Validators.maxLength(64)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    });
  }

  loadProfile() {
    this.isLoading = true;
    this.profileService.getProfile().subscribe({
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
    if (this.form.invalid) return;

    this.isLoading = true;
    const input: UpdateProfileDto = this.form.value;

    this.profileService.updateProfile(input).subscribe({
      next: () => {
        this.toaster.success('Profile updated successfully.', 'Success');
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error(err.error?.error?.message || 'An error occurred.', 'Error');
        this.isLoading = false;
      }
    });
  }
}
