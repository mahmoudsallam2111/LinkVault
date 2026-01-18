using Xunit;

namespace LinkVault.EntityFrameworkCore;

[CollectionDefinition(LinkVaultTestConsts.CollectionDefinitionName)]
public class LinkVaultEntityFrameworkCoreCollection : ICollectionFixture<LinkVaultEntityFrameworkCoreFixture>
{

}
