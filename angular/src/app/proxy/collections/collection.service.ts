import type { CollectionDto, CreateUpdateCollectionDto, ReorderCollectionDto, PublicCollectionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { ListResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CollectionService {
  private restService = inject(RestService);
  apiName = 'Default';


  create = (input: CreateUpdateCollectionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CollectionDto>({
      method: 'POST',
      url: '/api/app/collection',
      body: input,
    },
      { apiName: this.apiName, ...config });


  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/collection/${id}`,
    },
      { apiName: this.apiName, ...config });


  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CollectionDto>({
      method: 'GET',
      url: `/api/app/collection/${id}`,
    },
      { apiName: this.apiName, ...config });


  getList = (filter?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<CollectionDto>>({
      method: 'GET',
      url: '/api/app/collection',
      params: { filter },
    },
      { apiName: this.apiName, ...config });


  getTree = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ListResultDto<CollectionDto>>({
      method: 'GET',
      url: '/api/app/collection/tree',
    },
      { apiName: this.apiName, ...config });


  move = (id: string, newParentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CollectionDto>({
      method: 'POST',
      url: `/api/app/collection/${id}/move/${newParentId}`,
    },
      { apiName: this.apiName, ...config });


  reorder = (items: ReorderCollectionDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/collection/reorder',
      body: items,
    },
      { apiName: this.apiName, ...config });


  update = (id: string, input: CreateUpdateCollectionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CollectionDto>({
      method: 'PUT',
      url: `/api/app/collection/${id}`,
      body: input,
    },
      { apiName: this.apiName, ...config });

  generateShareToken = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CollectionDto>({
      method: 'POST',
      url: `/api/app/collection/${id}/generate-share-token`,
    },
      { apiName: this.apiName, ...config });

  revokeShareToken = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/collection/${id}/revoke-share-token`,
    },
      { apiName: this.apiName, ...config });

  getByShareToken = (token: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicCollectionDto>({
      method: 'GET',
      url: '/api/app/collection/by-share-token',
      params: { token },
    },
      { apiName: this.apiName, ...config });
}