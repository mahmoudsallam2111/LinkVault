import type { ImportBookmarkDto, ImportResultDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  import = (bookmarks: ImportBookmarkDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, ImportResultDto>({
      method: 'POST',
      url: '/api/app/import/import',
      body: bookmarks,
    },
    { apiName: this.apiName,...config });
  

  parseHtml = (htmlContent: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ImportBookmarkDto[]>({
      method: 'POST',
      url: '/api/app/import/parse-html',
      params: { htmlContent },
    },
    { apiName: this.apiName,...config });
  

  parseJson = (jsonContent: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ImportBookmarkDto[]>({
      method: 'POST',
      url: '/api/app/import/parse-json',
      params: { jsonContent },
    },
    { apiName: this.apiName,...config });
}