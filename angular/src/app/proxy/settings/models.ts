
export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  newPasswordConfirm: string;
}

export interface EmailPreferencesDto {
  newsletter?: boolean;
  linkSharing?: boolean;
  securityAlerts?: boolean;
  weeklyDigest?: boolean;
}

export interface UpdateProfileDto {
  userName: string;
  name?: string;
  surname?: string;
  email: string;
}
