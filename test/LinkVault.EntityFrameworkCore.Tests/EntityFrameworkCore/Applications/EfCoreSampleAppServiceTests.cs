using LinkVault.Samples;
using Xunit;

namespace LinkVault.EntityFrameworkCore.Applications;

[Collection(LinkVaultTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<LinkVaultEntityFrameworkCoreTestModule>
{

}
