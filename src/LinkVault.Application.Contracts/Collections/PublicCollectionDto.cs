using System;
using System.Collections.Generic;
using LinkVault.Links;

namespace LinkVault.Collections;

/// <summary>
/// DTO for displaying collection information to anonymous users.
/// Contains only necessary fields for public view.
/// </summary>
public class PublicCollectionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int LinkCount { get; set; }
    public List<PublicLinkDto> Links { get; set; } = new();
}

/// <summary>
/// DTO for displaying link information to anonymous users.
/// Contains only necessary fields for public view.
/// </summary>
public class PublicLinkDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FaviconUrl { get; set; }
}
