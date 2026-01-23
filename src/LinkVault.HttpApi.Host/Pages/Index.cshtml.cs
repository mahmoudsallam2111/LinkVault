using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Localization;
using Volo.Abp.OpenIddict.Applications;

namespace LinkVault.Pages;

public class IndexModel : AbpPageModel
{
    public List<OpenIddictApplication>? Applications { get; protected set; }

    public IReadOnlyList<LanguageInfo>? Languages { get; protected set; }

    public string? CurrentLanguage { get; protected set; }

    protected IOpenIddictApplicationRepository OpenIdApplicationRepository { get; }

    protected ILanguageProvider LanguageProvider { get; }
    
    protected IConfiguration Configuration { get; }

    public IndexModel(
        IOpenIddictApplicationRepository openIdApplicationmRepository, 
        ILanguageProvider languageProvider,
        IConfiguration configuration)
    {
        OpenIdApplicationRepository = openIdApplicationmRepository;
        LanguageProvider = languageProvider;
        Configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Auto-redirect authenticated users to Angular app
        if (CurrentUser.IsAuthenticated)
        {
            var angularUrl = Configuration["App:AngularUrl"];
            if (!string.IsNullOrEmpty(angularUrl))
            {
                return Redirect(angularUrl);
            }
        }
        
        Applications = await OpenIdApplicationRepository.GetListAsync();

        Languages = await LanguageProvider.GetLanguagesAsync();
        CurrentLanguage = CultureInfo.CurrentCulture.DisplayName;
        
        return Page();
    }
}

