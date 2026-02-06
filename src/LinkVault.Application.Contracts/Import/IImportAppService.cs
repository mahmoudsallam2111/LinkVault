using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace LinkVault.Import;

public interface IImportAppService : IApplicationService
{
    Task<ImportResultDto> ImportAsync(List<ImportBookmarkDto> bookmarks);

    Task<List<ImportBookmarkDto>> ParseHtmlAsync(string htmlContent);
    Task<List<ImportBookmarkDto>> ParseJsonAsync(string jsonContent);
}
