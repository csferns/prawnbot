using Discord.Commands;
using Discord.WebSocket;
using System;

namespace Prawnbot.Core.BusinessLayer
{
    public class BaseBL
    {
        public static DiscordSocketClient Client { get; set; }
        public static SocketCommandContext Context { get; set; }
    }
}
