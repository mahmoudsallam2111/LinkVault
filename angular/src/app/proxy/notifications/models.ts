import type { EntityDto, PagedResultRequestDto } from '@abp/ng.core';

export interface AppNotificationDto extends EntityDto<string> {
  title?: string;
  message?: string;
  url?: string;
  isRead?: boolean;
  icon?: string;
  creationTime?: string;
}

export interface GetNotificationListDto extends PagedResultRequestDto {
  unreadOnly?: boolean;
}
