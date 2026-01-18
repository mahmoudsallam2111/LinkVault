using System.ComponentModel.DataAnnotations;

namespace LinkVault.Tags;

/// <summary>
/// DTO for creating or updating a tag.
/// </summary>
public class CreateUpdateTagDto
{
    [Required]
    [StringLength(TagConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(TagConsts.MaxColorLength)]
    public string Color { get; set; } = TagConsts.DefaultColor;
}
