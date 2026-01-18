using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Tags;

/// <summary>
/// Application service interface for managing tags.
/// </summary>
public interface ITagAppService : IApplicationService
{
    /// <summary>
    /// Gets a tag by ID.
    /// </summary>
    Task<TagDto> GetAsync(Guid id);

    /// <summary>
    /// Gets all tags for the current user.
    /// </summary>
    Task<ListResultDto<TagDto>> GetListAsync(string? filter = null);

    /// <summary>
    /// Creates a new tag.
    /// </summary>
    Task<TagDto> CreateAsync(CreateUpdateTagDto input);

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    Task<TagDto> UpdateAsync(Guid id, CreateUpdateTagDto input);

    /// <summary>
    /// Deletes a tag.
    /// </summary>
    Task DeleteAsync(Guid id);
}
