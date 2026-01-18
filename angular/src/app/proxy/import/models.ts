
export interface ImportBookmarkDto {
  url?: string;
  title?: string;
  description?: string;
  folderPath?: string;
  tags?: string[];
}

export interface ImportResultDto {
  totalProcessed?: number;
  successCount?: number;
  duplicateCount?: number;
  failedCount?: number;
  errors?: string[];
}
