import type { CreateUpdateLinkDto, LinkDto, LinkFilterDto, LinkMetadataDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto, PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LinkService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateLinkDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'POST',
      url: '/api/app/link',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createFromUrl = (url: string, collectionId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'POST',
      url: '/api/app/link/from-url',
      params: { url, collectionId },
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/link/${id}`,
    },
    { apiName: this.apiName,...config });
  

  fetchMetadata = (url: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkMetadataDto>({
      method: 'POST',
      url: '/api/app/link/fetch-metadata',
      params: { url },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'GET',
      url: `/api/app/link/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAddedToday = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<LinkDto>>({
      method: 'GET',
      url: '/api/app/link/added-today',
    },
    { apiName: this.apiName,...config });
  

  getDomains = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<string>>({
      method: 'GET',
      url: '/api/app/link/domains',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: LinkFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LinkDto>>({
      method: 'GET',
      url: '/api/app/link',
      params: { filter: input.filter, collectionId: input.collectionId, tagIds: input.tagIds, domain: input.domain, isFavorite: input.isFavorite, includeDeleted: input.includeDeleted, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getTrash = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LinkDto>>({
      method: 'GET',
      url: '/api/app/link/trash',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  hardDelete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/link/${id}/hard-delete`,
    },
    { apiName: this.apiName,...config });
  

  incrementVisit = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'POST',
      url: `/api/app/link/${id}/increment-visit`,
    },
    { apiName: this.apiName,...config });
  

  restore = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/link/${id}/restore`,
    },
    { apiName: this.apiName,...config });
  

  toggleFavorite = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'POST',
      url: `/api/app/link/${id}/toggle-favorite`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateLinkDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LinkDto>({
      method: 'PUT',
      url: `/api/app/link/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}