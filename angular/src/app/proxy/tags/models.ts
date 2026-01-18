import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateTagDto {
  name: string;
  color?: string;
}

export interface TagDto extends FullAuditedEntityDto<string> {
  userId?: string;
  name?: string;
  color?: string;
  linkCount?: number;
}
