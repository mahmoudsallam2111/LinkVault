import type { CreateUpdateTagDto, TagDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateTagDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'POST',
      url: '/api/app/tag',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tag/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'GET',
      url: `/api/app/tag/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (filter?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<TagDto>>({
      method: 'GET',
      url: '/api/app/tag',
      params: { filter },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateTagDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TagDto>({
      method: 'PUT',
      url: `/api/app/tag/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}