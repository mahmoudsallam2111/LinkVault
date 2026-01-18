import type { ChangePasswordDto, EmailPreferencesDto, UpdateProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  changePassword = (input: ChangePasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/profile/change-password',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteAccount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/profile/account',
    },
    { apiName: this.apiName,...config });
  

  getEmailPreferences = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmailPreferencesDto>({
      method: 'GET',
      url: '/api/app/profile/email-preferences',
    },
    { apiName: this.apiName,...config });
  

  getProfile = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UpdateProfileDto>({
      method: 'GET',
      url: '/api/app/profile/profile',
    },
    { apiName: this.apiName,...config });
  

  updateEmailPreferences = (input: EmailPreferencesDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/profile/email-preferences',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateProfile = (input: UpdateProfileDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/profile/profile',
      body: input,
    },
    { apiName: this.apiName,...config });
}