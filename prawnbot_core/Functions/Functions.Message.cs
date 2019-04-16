using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Functions
    {
        public async Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText)
        {
            await GetGuildTextChannel(guild, channel).SendMessageAsync(messageText);
        }

        public async Task SendDMAsync(SocketGuildUser user, string messageText)
        {
            await UserExtensions.SendMessageAsync(user, messageText);
        }
    }
}
