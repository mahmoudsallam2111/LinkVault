using System;
using System.ComponentModel.DataAnnotations;

namespace LinkVault.Collections;

/// <summary>
/// DTO for creating or updating a collection.
/// </summary>
public class CreateUpdateCollectionDto
{
    [Required]
    [StringLength(CollectionConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(CollectionConsts.MaxColorLength)]
    public string Color { get; set; } = CollectionConsts.DefaultColor;

    [StringLength(CollectionConsts.MaxIconLength)]
    public string? Icon { get; set; }

    public Guid? ParentId { get; set; }

    public int Order { get; set; }
}
