import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateLinkDto {
  url: string;
  title: string;
  description?: string;
  favicon?: string;
  isFavorite?: boolean;
  collectionId?: string;
  tagNames?: string[];
}

export interface LinkDto extends FullAuditedEntityDto<string> {
  userId?: string;
  url?: string;
  title?: string;
  description?: string;
  favicon?: string;
  domain?: string;
  isFavorite?: boolean;
  visitCount?: number;
  collectionId?: string;
  collectionName?: string;
  tags?: LinkTagDto[];
}

export interface LinkFilterDto extends PagedAndSortedResultRequestDto {
  filter?: string;
  collectionId?: string;
  tagIds?: string[];
  domain?: string;
  isFavorite?: boolean;
  includeDeleted?: boolean;
}

export interface LinkMetadataDto {
  title?: string;
  description?: string;
  favicon?: string;
}

export interface LinkTagDto {
  id?: string;
  name?: string;
  color?: string;
}
