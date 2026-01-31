import type { CreationAuditedEntityDto } from '@abp/ng.core';

export interface CreateLinkReminderDto {
  linkId: string;
  remindAt?: string;
  durationHours?: number;
  note?: string;
}

export interface LinkReminderDto extends CreationAuditedEntityDto<string> {
  userId?: string;
  linkId?: string;
  remindAt?: string;
  isTriggered?: boolean;
  triggeredAt?: string;
  note?: string;
  linkTitle?: string;
  linkUrl?: string;
  linkFavicon?: string;
  linkDomain?: string;
}

export interface UserReminderSettingsDto {
  defaultReminderHours?: number;
  enableInAppNotifications?: boolean;
  enableEmailNotifications?: boolean;
}
