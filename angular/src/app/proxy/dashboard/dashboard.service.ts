import type { DashboardStatsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { LinkDto } from '../links/models';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getMostVisited = (count: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto[]>({
      method: 'GET',
      url: '/api/app/dashboard/most-visited',
      params: { count },
    },
    { apiName: this.apiName,...config });
  

  getRecent = (count: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto[]>({
      method: 'GET',
      url: '/api/app/dashboard/recent',
      params: { count },
    },
    { apiName: this.apiName,...config });
  

  getStats = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DashboardStatsDto>({
      method: 'GET',
      url: '/api/app/dashboard/stats',
    },
    { apiName: this.apiName,...config });
}