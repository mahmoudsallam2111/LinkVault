using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Links;

/// <summary>
/// Application service interface for managing links.
/// </summary>
public interface ILinkAppService : IApplicationService
{
    /// <summary>
    /// Gets a link by ID.
    /// </summary>
    Task<LinkDto> GetAsync(Guid id);

    /// <summary>
    /// Gets a paged list of links with filtering.
    /// </summary>
    Task<PagedResultDto<LinkDto>> GetListAsync(LinkFilterDto input);

    /// <summary>
    /// Creates a new link with optional metadata fetching.
    /// </summary>
    Task<LinkDto> CreateAsync(CreateUpdateLinkDto input);

    /// <summary>
    /// Creates a link by just providing URL - fetches metadata automatically.
    /// </summary>
    Task<LinkDto> CreateFromUrlAsync(string url, Guid? collectionId = null);

    /// <summary>
    /// Updates an existing link.
    /// </summary>
    Task<LinkDto> UpdateAsync(Guid id, CreateUpdateLinkDto input);

    /// <summary>
    /// Soft deletes a link (moves to trash).
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Restores a soft-deleted link from trash.
    /// </summary>
    Task RestoreAsync(Guid id);

    /// <summary>
    /// Permanently deletes a link.
    /// </summary>
    Task HardDeleteAsync(Guid id);

    /// <summary>
    /// Toggles the favorite status of a link.
    /// </summary>
    Task<LinkDto> ToggleFavoriteAsync(Guid id);

    /// <summary>
    /// Increments the visit count and returns the updated link.
    /// </summary>
    Task<LinkDto> IncrementVisitAsync(Guid id);

    /// <summary>
    /// Gets deleted links (trash).
    /// </summary>
    Task<PagedResultDto<LinkDto>> GetTrashAsync(PagedAndSortedResultRequestDto input);

    /// <summary>
    /// Fetches metadata for a URL without saving.
    /// </summary>
    Task<LinkMetadataDto> FetchMetadataAsync(string url);

    /// <summary>
    /// Gets all unique domains for the current user.
    /// </summary>
    Task<ListResultDto<string>> GetDomainsAsync();

    /// <summary>
    /// Gets links added today.
    /// </summary>
    Task<ListResultDto<LinkDto>> GetAddedTodayAsync();
}

/// <summary>
/// DTO for URL metadata.
/// </summary>
public class LinkMetadataDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Favicon { get; set; }
}
