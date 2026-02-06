using System;
using System.Collections.Generic;

namespace LinkVault.Collections;

public class PublicCollectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int LinkCount { get; set; }
    public List<PublicLinkDto> Links { get; set; } = new();
}

public class PublicLinkDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FaviconUrl { get; set; }
}
