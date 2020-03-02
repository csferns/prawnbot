using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Infrastructure.Interfaces;

namespace Prawnbot.Infrastructure
{
    /// <summary>
    /// Base class for all the Business Layers that includes shared properties needed throughout the Business Layers
    /// </summary>
    public class BaseBL : IBaseBL
    {
        public static DiscordSocketClient Client { get; set; }
        public static SocketCommandContext Context { get; set; }

        public bool UseTTS { get; set; }
    }
}
