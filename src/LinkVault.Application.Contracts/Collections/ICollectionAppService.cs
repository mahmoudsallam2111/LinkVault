using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Collections;


public interface ICollectionAppService : IApplicationService
{
   
    Task<CollectionDto> GetAsync(Guid id);

    Task<ListResultDto<CollectionDto>> GetListAsync(string? filter = null);

    Task<ListResultDto<CollectionDto>> GetTreeAsync();

    Task<CollectionDto> CreateAsync(CreateUpdateCollectionDto input);

    Task<CollectionDto> UpdateAsync(Guid id, CreateUpdateCollectionDto input);

    Task DeleteAsync(Guid id);

    Task<CollectionDto> MoveAsync(Guid id, Guid? newParentId);
    Task ReorderAsync(List<ReorderCollectionDto> items);

    Task<CollectionDto> GenerateShareTokenAsync(Guid id);

    Task RevokeShareTokenAsync(Guid id);

    Task<PublicCollectionDto?> GetByShareTokenAsync(string token);
}

public class ReorderCollectionDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}
