using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LinkVault.Import;

/// <summary>
/// Application service interface for importing bookmarks.
/// </summary>
public interface IImportAppService : IApplicationService
{
    /// <summary>
    /// Imports bookmarks from a list of bookmark DTOs.
    /// </summary>
    Task<ImportResultDto> ImportAsync(List<ImportBookmarkDto> bookmarks);

    /// <summary>
    /// Parses an HTML bookmark file (Netscape format) and returns bookmark DTOs.
    /// </summary>
    Task<List<ImportBookmarkDto>> ParseHtmlAsync(string htmlContent);

    /// <summary>
    /// Parses a JSON bookmark file and returns bookmark DTOs.
    /// </summary>
    Task<List<ImportBookmarkDto>> ParseJsonAsync(string jsonContent);
}
