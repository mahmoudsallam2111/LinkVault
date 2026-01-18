import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CollectionDto extends FullAuditedEntityDto<string> {
  userId?: string;
  parentId?: string;
  name?: string;
  color?: string;
  icon?: string;
  order?: number;
  linkCount?: number;
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
