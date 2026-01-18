using LinkVault.Samples;
using Xunit;

namespace LinkVault.EntityFrameworkCore.Domains;

[Collection(LinkVaultTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<LinkVaultEntityFrameworkCoreTestModule>
{

}
