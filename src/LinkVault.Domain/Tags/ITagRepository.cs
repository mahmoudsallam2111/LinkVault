using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Tags;

/// <summary>
/// Custom repository interface for Tag entity.
/// </summary>
public interface ITagRepository : IRepository<Tag, Guid>
{
    /// <summary>
    /// Gets all tags for a user.
    /// </summary>
    Task<List<Tag>> GetListAsync(
        Guid userId,
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a tag name already exists for a user.
    /// </summary>
    Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tags with their link counts.
    /// </summary>
    Task<List<TagWithCountDto>> GetWithLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates tags by name for a user.
    /// </summary>
    Task<List<Tag>> GetOrCreateByNamesAsync(
        Guid userId,
        List<string> names,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO for tag with link count.
/// </summary>
public class TagWithCountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int LinkCount { get; set; }
}
