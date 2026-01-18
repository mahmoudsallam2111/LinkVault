using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Collections;
using LinkVault.Links;
using LinkVault.Permissions;
using LinkVault.Tags;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace LinkVault.Dashboard;

[AllowAnonymous]
//[Authorize]
public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ITagRepository _tagRepository;

    public DashboardAppService(
        ILinkRepository linkRepository,
        ICollectionRepository collectionRepository,
        ITagRepository tagRepository)
    {
        _linkRepository = linkRepository;
        _collectionRepository = collectionRepository;
        _tagRepository = tagRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var userId = CurrentUser.Id!.Value;

        var linkStats = await _linkRepository.GetStatsAsync(userId);
        var collections = await _collectionRepository.GetListAsync(userId);
        var tags = await _tagRepository.GetListAsync(userId);

        return new DashboardStatsDto
        {
            TotalLinks = linkStats.TotalLinks,
            FavoriteCount = linkStats.FavoriteCount,
            TotalClicks = linkStats.TotalClicks,
            LinksAddedThisWeek = linkStats.LinksAddedThisWeek,
            CollectionCount = collections.Count,
            TagCount = tags.Count,
            LinksPerCollection = linkStats.LinksPerCollection
        };
    }

    public async Task<List<LinkDto>> GetMostVisitedAsync(int count = 10)
    {
        var userId = CurrentUser.Id!.Value;
        var links = await _linkRepository.GetMostVisitedAsync(userId, count);
        return links.Select(MapToDto).ToList();
    }

    public async Task<List<LinkDto>> GetRecentAsync(int count = 10)
    {
        var userId = CurrentUser.Id!.Value;
        var links = await _linkRepository.GetRecentAsync(userId, count);
        return links.Select(MapToDto).ToList();
    }

    private static LinkDto MapToDto(Link link)
    {
        return new LinkDto
        {
            Id = link.Id,
            UserId = link.UserId,
            Url = link.Url,
            Title = link.Title,
            Description = link.Description,
            Favicon = link.Favicon,
            Domain = link.Domain,
            IsFavorite = link.IsFavorite,
            VisitCount = link.VisitCount,
            CollectionId = link.CollectionId,
            CollectionName = link.Collection?.Name,
            CreationTime = link.CreationTime,
            LastModificationTime = link.LastModificationTime
        };
    }
}
