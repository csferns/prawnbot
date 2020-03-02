using Discord;
using Discord.WebSocket;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using Quartz;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class SeasonalEmojiJob : IJob
    {
        private readonly ICoreService coreService;

        public SeasonalEmojiJob(ICoreService coreService)
        {
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ListResponse<SocketGuild> guildsResponse = coreService.GetAllGuilds();

            if (guildsResponse.HasData)
            {
                Bunch<SocketGuild> guilds = guildsResponse.Entities.ToBunch();

                Emoji textChannelEmoji = new Emoji("");
                Emoji categoryEmoji = new Emoji("");
                Emoji voiceChannelEmoji = new Emoji("");

                foreach (SocketGuild guild in guilds)
                {
                    // Text channels
                    foreach (SocketGuildChannel channel in guild.TextChannels)
                    {
                        await channel.ModifyAsync(opt => opt.Name = $":{textChannelEmoji.Name}:" + " " + opt.Name);
                    }

                    // Category channels
                    foreach (SocketCategoryChannel channel in guild.CategoryChannels)
                    {
                        await channel.ModifyAsync(opt => opt.Name = $":{categoryEmoji.Name}:" + " " + opt.Name);
                    }

                    // Voice channels
                    foreach (SocketVoiceChannel channel in guild.VoiceChannels)
                    {
                        await channel.ModifyAsync(opt => opt.Name = $":{voiceChannelEmoji.Name}:" + " " + opt.Name);
                    }
                }
            }
        }
    }
}
