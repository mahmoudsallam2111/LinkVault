import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileService, ChangePasswordDto } from '../../proxy/settings';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './password.html',
  styleUrl: './password.scss'
})
export class Password implements OnInit {
  form: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.buildForm();
  }

  buildForm() {
    this.form = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      newPasswordConfirm: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('newPasswordConfirm')?.value
      ? null : { mismatch: true };
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    const input: ChangePasswordDto = this.form.value;

    this.profileService.changePassword(input).subscribe({
      next: () => {
        this.toaster.success('Password changed successfully.', 'Success');
        this.form.reset();
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error(err.error?.error?.message || 'Failed to change password.', 'Error');
        this.isLoading = false;
      }
    });
  }
}
