using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.Module
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("impersonate-user")]
        public async Task ImpersonateUser(SocketGuildUser user)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("No user passed in!");
                return;
            }

            RequestOptions options = new RequestOptions
            {
                RetryMode = RetryMode.RetryTimeouts,
                Timeout = 1000,
                CancelToken = CancellationToken.None
            };

            IReadOnlyCollection<RestWebhook> webhooks = await Context.Guild.GetWebhooksAsync();

            RestWebhook prawnbotWebhook = webhooks.FirstOrDefault(x => x.Name == "PrawnBot");

            if (prawnbotWebhook != default(RestWebhook))
            {
                //RestWebhook restClient = new RestWebhook(prawnbotWebhook);
            }
        }
    }
}
