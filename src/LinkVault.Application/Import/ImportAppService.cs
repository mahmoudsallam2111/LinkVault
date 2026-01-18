using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinkVault.Collections;
using LinkVault.Links;
using LinkVault.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace LinkVault.Import;

[AllowAnonymous]
//[Authorize(LinkVaultPermissions.Import.Default)]
public class ImportAppService : ApplicationService, IImportAppService
{
    private readonly ILinkRepository _linkRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly LinkManager _linkManager;

    public ImportAppService(
        ILinkRepository linkRepository,
        ICollectionRepository collectionRepository,
        LinkManager linkManager)
    {
        _linkRepository = linkRepository;
        _collectionRepository = collectionRepository;
        _linkManager = linkManager;
    }

    public async Task<ImportResultDto> ImportAsync(List<ImportBookmarkDto> bookmarks)
    {
        var userId = CurrentUser.Id!.Value;
        var result = new ImportResultDto();

        // Cache collections for folder mapping
        var collectionCache = new Dictionary<string, Guid>();

        foreach (var bookmark in bookmarks)
        {
            result.TotalProcessed++;

            try
            {
                // Check for duplicate
                if (await _linkRepository.ExistsAsync(userId, bookmark.Url))
                {
                    result.DuplicateCount++;
                    continue;
                }

                // Get or create collection for folder path
                Guid? collectionId = null;
                if (!string.IsNullOrWhiteSpace(bookmark.FolderPath))
                {
                    collectionId = await GetOrCreateCollectionAsync(userId, bookmark.FolderPath, collectionCache);
                }

                var title = string.IsNullOrWhiteSpace(bookmark.Title) ? bookmark.Url : bookmark.Title;
                var link = await _linkManager.CreateAsync(
                    userId,
                    bookmark.Url,
                    title,
                    bookmark.Description,
                    null,
                    collectionId);

                await _linkRepository.InsertAsync(link);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Failed to import {bookmark.Url}: {ex.Message}");
            }
        }

        return result;
    }

    public Task<List<ImportBookmarkDto>> ParseHtmlAsync(string htmlContent)
    {
        var bookmarks = new List<ImportBookmarkDto>();
        var currentPath = new Stack<string>();

        // Parse Netscape bookmark format
        // Find all <DT><A HREF="...">Title</A> and <DT><H3>Folder</H3> tags

        // Track folder hierarchy
        var lines = htmlContent.Split('\n');
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Check for folder start
            var folderMatch = Regex.Match(trimmedLine, @"<H3[^>]*>([^<]+)</H3>", RegexOptions.IgnoreCase);
            if (folderMatch.Success)
            {
                currentPath.Push(folderMatch.Groups[1].Value.Trim());
                continue;
            }

            // Check for folder end
            if (trimmedLine.Equals("</DL><p>", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Equals("</DL>", StringComparison.OrdinalIgnoreCase))
            {
                if (currentPath.Count > 0)
                {
                    currentPath.Pop();
                }
                continue;
            }

            // Check for bookmark
            var bookmarkMatch = Regex.Match(trimmedLine, 
                @"<A\s+HREF=""([^""]+)""[^>]*>([^<]*)</A>", 
                RegexOptions.IgnoreCase);
            
            if (bookmarkMatch.Success)
            {
                var url = bookmarkMatch.Groups[1].Value;
                var title = System.Net.WebUtility.HtmlDecode(bookmarkMatch.Groups[2].Value);

                if (Uri.TryCreate(url, UriKind.Absolute, out _))
                {
                    bookmarks.Add(new ImportBookmarkDto
                    {
                        Url = url,
                        Title = title,
                        FolderPath = string.Join("/", currentPath.Reverse())
                    });
                }
            }
        }

        return Task.FromResult(bookmarks);
    }

    public Task<List<ImportBookmarkDto>> ParseJsonAsync(string jsonContent)
    {
        var bookmarks = new List<ImportBookmarkDto>();

        try
        {
            using var doc = JsonDocument.Parse(jsonContent);
            ParseJsonNode(doc.RootElement, bookmarks, "");
        }
        catch
        {
            throw new BusinessException(LinkVaultDomainErrorCodes.InvalidImportFormat);
        }

        return Task.FromResult(bookmarks);
    }

    private void ParseJsonNode(JsonElement element, List<ImportBookmarkDto> bookmarks, string path)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            // Chrome/Firefox bookmark format
            if (element.TryGetProperty("type", out var typeElement))
            {
                var type = typeElement.GetString();
                
                if (type == "url" && element.TryGetProperty("url", out var urlElement))
                {
                    var url = urlElement.GetString();
                    var title = element.TryGetProperty("name", out var nameElement) 
                        ? nameElement.GetString() 
                        : url;

                    if (!string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _))
                    {
                        bookmarks.Add(new ImportBookmarkDto
                        {
                            Url = url!,
                            Title = title ?? url!,
                            FolderPath = path
                        });
                    }
                }
                else if (type == "folder")
                {
                    var folderName = element.TryGetProperty("name", out var nameElement) 
                        ? nameElement.GetString() 
                        : "Folder";
                    
                    var newPath = string.IsNullOrEmpty(path) ? folderName : $"{path}/{folderName}";

                    if (element.TryGetProperty("children", out var childrenElement))
                    {
                        ParseJsonNode(childrenElement, bookmarks, newPath ?? path);
                    }
                }
            }

            // Also check for direct children property
            if (element.TryGetProperty("children", out var children))
            {
                ParseJsonNode(children, bookmarks, path);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                ParseJsonNode(item, bookmarks, path);
            }
        }
    }

    private async Task<Guid> GetOrCreateCollectionAsync(
        Guid userId, 
        string folderPath, 
        Dictionary<string, Guid> cache)
    {
        if (cache.TryGetValue(folderPath, out var cachedId))
        {
            return cachedId;
        }

        var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        Guid? parentId = null;
        var currentPath = "";

        foreach (var part in parts)
        {
            currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}/{part}";

            if (cache.TryGetValue(currentPath, out var existingId))
            {
                parentId = existingId;
                continue;
            }

            // Check if collection exists
            var existing = (await _collectionRepository.GetListAsync(userId))
                .FirstOrDefault(c => c.Name == part && c.ParentId == parentId);

            if (existing != null)
            {
                cache[currentPath] = existing.Id;
                parentId = existing.Id;
            }
            else
            {
                // Create new collection
                var collection = new Collection(
                    GuidGenerator.Create(),
                    userId,
                    part,
                    parentId);

                await _collectionRepository.InsertAsync(collection, autoSave: true);
                cache[currentPath] = collection.Id;
                parentId = collection.Id;
            }
        }

        return parentId!.Value;
    }
}
