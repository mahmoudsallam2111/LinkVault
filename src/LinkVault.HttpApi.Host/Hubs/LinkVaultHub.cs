using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;

namespace LinkVault.Hubs;

[Authorize]
[HubRoute("/signalr-hubs/link-vault")]
public class LinkVaultHub : AbpHub
{
    // We can add methods here if the client needs to invoke server methods.
    // For now, we only use it to push notifications from server to client.
}
