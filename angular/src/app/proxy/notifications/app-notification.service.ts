import type { AppNotificationDto, GetNotificationListDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppNotificationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetNotificationListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<AppNotificationDto>>({
      method: 'GET',
      url: '/api/app/app-notification',
      params: { unreadOnly: input.unreadOnly, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getUnreadCount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/app-notification/unread-count',
    },
    { apiName: this.apiName,...config });
  

  markAllAsRead = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/app-notification/mark-all-as-read',
    },
    { apiName: this.apiName,...config });
  

  markAsRead = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/app-notification/${id}/mark-as-read`,
    },
    { apiName: this.apiName,...config });
}