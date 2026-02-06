using System.Collections.Generic;

namespace LinkVault.Import;

public class ImportBookmarkDto
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FolderPath { get; set; }
    public List<string> Tags { get; set; } = new();
}
public class ImportResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int DuplicateCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
