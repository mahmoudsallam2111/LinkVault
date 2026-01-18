import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileService } from '../../proxy/settings';
import { ToasterService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';

@Component({
  selector: 'app-account-deletion',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './account-deletion.html',
  styleUrl: './account-deletion.scss'
})
export class AccountDeletion {
  confirmationStr = '';
  isLoading = false;

  constructor(
    private profileService: ProfileService,
    private toaster: ToasterService,
    private authService: AuthService
  ) { }

  onDelete() {
    if (this.confirmationStr !== 'DELETE') {
      this.toaster.error('Please type DELETE to confirm.', 'Error');
      return;
    }

    if (!confirm('Are you absolutely sure you want to delete your account? This action cannot be undone.')) {
      return;
    }

    this.isLoading = true;
    this.profileService.deleteAccount().subscribe({
      next: () => {
        this.toaster.success('Account deleted successfully.', 'Goodbye');
        this.authService.logout().subscribe();
      },
      error: (err) => {
        this.toaster.error('Failed to delete account.', 'Error');
        this.isLoading = false;
      }
    });
  }
}
