using LinkVault.Links.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace LinkVault.Links;

public interface ILinkAppService : IApplicationService
{
    Task<LinkDto> GetAsync(Guid id);

    Task<PagedResultDto<LinkDto>> GetListAsync(LinkFilterDto input);

    Task<LinkDto> CreateAsync(CreateUpdateLinkDto input);

    Task<LinkDto> CreateFromUrlAsync(string url, Guid? collectionId = null);

    Task<LinkDto> UpdateAsync(Guid id, CreateUpdateLinkDto input);

    Task DeleteAsync(Guid id);

    Task RestoreAsync(Guid id);
    Task HardDeleteAsync(Guid id);

    Task<LinkDto> ToggleFavoriteAsync(Guid id);

    Task<LinkDto> IncrementVisitAsync(Guid id);

    Task<PagedResultDto<LinkDto>> GetTrashAsync(PagedAndSortedResultRequestDto input);

    Task<LinkMetadataDto> FetchMetadataAsync(string url);

    Task<ListResultDto<string>> GetDomainsAsync();

    Task<ListResultDto<LinkDto>> GetAddedThisWeekAsync();
}
