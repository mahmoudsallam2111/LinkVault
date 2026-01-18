using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace LinkVault.Links;

/// <summary>
/// Domain service for managing Link entities.
/// Contains business logic for link creation and validation.
/// </summary>
public class LinkManager : DomainService
{
    private readonly ILinkRepository _linkRepository;

    public LinkManager(ILinkRepository linkRepository)
    {
        _linkRepository = linkRepository;
    }

    /// <summary>
    /// Creates a new link with validation.
    /// </summary>
    /// <param name="userId">The owner user ID.</param>
    /// <param name="url">The URL of the link.</param>
    /// <param name="title">The title of the link.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="favicon">Optional favicon URL.</param>
    /// <param name="collectionId">Optional collection ID.</param>
    /// <returns>The created Link entity.</returns>
    public async Task<Link> CreateAsync(
        Guid userId,
        string url,
        string title,
        string? description = null,
        string? favicon = null,
        Guid? collectionId = null)
    {
        Check.NotNullOrWhiteSpace(url, nameof(url));
        Check.NotNullOrWhiteSpace(title, nameof(title));

        // Check for duplicate URL
        if (await _linkRepository.ExistsAsync(userId, url))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateUrl)
                .WithData("url", url);
        }

        var domain = ExtractDomain(url);

        var link = new Link(
            GuidGenerator.Create(),
            userId,
            url,
            title,
            domain)
        {
            Description = description,
            Favicon = favicon,
            CollectionId = collectionId
        };

        return link;
    }

    /// <summary>
    /// Updates a link with duplicate URL validation.
    /// </summary>
    public async Task<Link> UpdateAsync(
        Link link,
        string url,
        string title,
        string? description,
        string? favicon,
        Guid? collectionId)
    {
        Check.NotNull(link, nameof(link));
        Check.NotNullOrWhiteSpace(url, nameof(url));
        Check.NotNullOrWhiteSpace(title, nameof(title));

        // Check for duplicate URL (excluding current link)
        if (link.Url != url && await _linkRepository.ExistsAsync(link.UserId, url, link.Id))
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.DuplicateUrl)
                .WithData("url", url);
        }

        if (link.Url != url)
        {
            link.SetUrl(url);
        }

        link.Title = title;
        link.Description = description;
        link.Favicon = favicon;
        link.CollectionId = collectionId;

        return link;
    }

    /// <summary>
    /// Extracts the domain from a URL.
    /// </summary>
    /// <param name="url">The URL to extract the domain from.</param>
    /// <returns>The domain (e.g., "github.com").</returns>
    public static string ExtractDomain(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Host.ToLowerInvariant();
        }
        catch
        {
            // If URL parsing fails, try to extract domain manually
            var urlLower = url.ToLowerInvariant();
            urlLower = urlLower.Replace("https://", "").Replace("http://", "");
            var slashIndex = urlLower.IndexOf('/');
            return slashIndex > 0 ? urlLower[..slashIndex] : urlLower;
        }
    }
}
