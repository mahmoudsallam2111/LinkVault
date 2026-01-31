using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace LinkVault.Links;

/// <summary>
/// Service for fetching metadata (title, description, favicon) from URLs.
/// </summary>
public class MetadataFetcherService : ITransientDependency
{
    private readonly ILogger<MetadataFetcherService> _logger;

    public MetadataFetcherService(
        ILogger<MetadataFetcherService> logger)
    {
        _logger = logger;
    }

    public async Task<LinkMetadata> FetchMetadataAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        var metadata = new LinkMetadata
        {
            Title = string.Empty,
            Description = null,
            Favicon = null
        };

        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "LinkVault/1.0 (Metadata Fetcher)");

            var response = await client.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch URL {Url}: {StatusCode}", url, response.StatusCode);
                return metadata;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            // Extract title
            metadata.Title = ExtractTitle(html);

            // Extract description
            metadata.Description = ExtractDescription(html);

            // Extract favicon
            metadata.Favicon = ExtractFavicon(html, url);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch metadata for URL: {Url}", url);
        }

        return metadata;
    }

    private static string ExtractTitle(string html)
    {
        // Try og:title first
        var ogTitleMatch = Regex.Match(html, @"<meta\s+property=[""']og:title[""']\s+content=[""']([^""']*)[""']", RegexOptions.IgnoreCase);
        if (ogTitleMatch.Success)
        {
            return DecodeHtmlEntities(ogTitleMatch.Groups[1].Value);
        }

        // Fallback to <title> tag
        var titleMatch = Regex.Match(html, @"<title[^>]*>([^<]*)</title>", RegexOptions.IgnoreCase);
        if (titleMatch.Success)
        {
            return DecodeHtmlEntities(titleMatch.Groups[1].Value.Trim());
        }

        return string.Empty;
    }

    private static string? ExtractDescription(string html)
    {
        // Try og:description first
        var ogDescMatch = Regex.Match(html, @"<meta\s+property=[""']og:description[""']\s+content=[""']([^""']*)[""']", RegexOptions.IgnoreCase);
        if (ogDescMatch.Success)
        {
            return DecodeHtmlEntities(ogDescMatch.Groups[1].Value);
        }

        // Fallback to meta description
        var descMatch = Regex.Match(html, @"<meta\s+name=[""']description[""']\s+content=[""']([^""']*)[""']", RegexOptions.IgnoreCase);
        if (descMatch.Success)
        {
            return DecodeHtmlEntities(descMatch.Groups[1].Value);
        }

        return null;
    }

    private static string? ExtractFavicon(string html, string url)
    {
        try
        {
            var uri = new Uri(url);
            var baseUrl = $"{uri.Scheme}://{uri.Host}";

            // Try to find favicon link in HTML
            var iconMatch = Regex.Match(html, @"<link[^>]+rel=[""'](?:shortcut )?icon[""'][^>]+href=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            if (!iconMatch.Success)
            {
                iconMatch = Regex.Match(html, @"<link[^>]+href=[""']([^""']+)[""'][^>]+rel=[""'](?:shortcut )?icon[""']", RegexOptions.IgnoreCase);
            }

            if (iconMatch.Success)
            {
                var favicon = iconMatch.Groups[1].Value;
                if (favicon.StartsWith("//"))
                {
                    return $"{uri.Scheme}:{favicon}";
                }
                if (favicon.StartsWith("/"))
                {
                    return $"{baseUrl}{favicon}";
                }
                if (!favicon.StartsWith("http"))
                {
                    return $"{baseUrl}/{favicon}";
                }
                return favicon;
            }

            // Default to /favicon.ico
            return $"{baseUrl}/favicon.ico";
        }
        catch
        {
            return null;
        }
    }

    private static string DecodeHtmlEntities(string text)
    {
        return System.Net.WebUtility.HtmlDecode(text);
    }
}
public class LinkMetadata
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Favicon { get; set; }
}
