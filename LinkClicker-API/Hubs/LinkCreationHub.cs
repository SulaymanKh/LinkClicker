using LinkClicker_API.Models.Admin;
using Microsoft.AspNetCore.SignalR;

namespace LinkClicker_API.Hubs
{
    public class LinkCreationHub : Hub
    {
        public async Task NotifyLinkCreationCompleted(Guid requestId, List<LinkInfoModel> links)
        {
            await Clients.All.SendAsync("LinkCreationCompleted", requestId, links);
        }
    }
}
