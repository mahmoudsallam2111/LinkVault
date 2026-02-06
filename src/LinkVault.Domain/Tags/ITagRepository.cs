using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LinkVault.Tags;

public interface ITagRepository : IRepository<Tag, Guid>
{
 
    Task<List<Tag>> GetListAsync(
        Guid userId,
        string? filter = null,
        CancellationToken cancellationToken = default);

 
    Task<bool> NameExistsAsync(
        Guid userId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

   
    Task<List<TagWithCountDto>> GetWithLinkCountsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<Tag>> GetOrCreateByNamesAsync(
        Guid userId,
        List<string> names,
        CancellationToken cancellationToken = default);
}

public class TagWithCountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int LinkCount { get; set; }
}
