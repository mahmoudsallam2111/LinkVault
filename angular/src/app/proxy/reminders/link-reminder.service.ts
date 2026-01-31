import type { CreateLinkReminderDto, LinkReminderDto, UserReminderSettingsDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LinkReminderService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateLinkReminderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkReminderDto>({
      method: 'POST',
      url: '/api/app/link-reminder',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/link-reminder/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByLink = (linkId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkReminderDto>({
      method: 'GET',
      url: `/api/app/link-reminder/by-link/${linkId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LinkReminderDto>>({
      method: 'GET',
      url: '/api/app/link-reminder',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPendingCount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/link-reminder/pending-count',
    },
    { apiName: this.apiName,...config });
  

  getSettings = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserReminderSettingsDto>({
      method: 'GET',
      url: '/api/app/link-reminder/settings',
    },
    { apiName: this.apiName,...config });
  

  updateSettings = (input: UserReminderSettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserReminderSettingsDto>({
      method: 'PUT',
      url: '/api/app/link-reminder/settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}