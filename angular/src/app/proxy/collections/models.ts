import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CollectionDto extends FullAuditedEntityDto<string> {
  userId?: string;
  parentId?: string;
  name?: string;
  color?: string;
  icon?: string;
  order?: number;
  linkCount?: number;
  publicShareToken?: string;
  children?: CollectionDto[];
}

export interface CreateUpdateCollectionDto {
  name: string;
  color?: string;
  icon?: string;
  parentId?: string;
  order?: number;
}

export interface ReorderCollectionDto {
  id?: string;
  order?: number;
}

export interface PublicCollectionDto {
  id?: string;
  name?: string;
  color?: string;
  icon?: string;
  linkCount?: number;
  links?: PublicLinkDto[];
}

export interface PublicLinkDto {
  id?: string;
  title?: string;
  url?: string;
  description?: string;
  faviconUrl?: string;
}
