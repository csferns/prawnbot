using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Quartz;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class MOCJob : IJob
    {
        private readonly ILogger<MOCJob> logger;
        private readonly ICoreService coreService;
        public MOCJob(ILogger<MOCJob> logger, ICoreService coreService)
        {
            this.logger = logger;
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (DateTime.Now.Date.ToString("dd/MM/yyyy") == $"11/09/{DateTime.Now.Year}")
                {
                    return;
                }

                Bunch<SocketGuild> guilds = coreService.GetAllGuilds().Entities.ToBunch();

                Random random = new Random();

                foreach (SocketGuild guild in guilds)
                {
                    int mocCount = random.Next(1, 10);

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm")}: {mocCount}");

                    for (int i = 0; i < mocCount; i++)
                    {
                        sb.AppendLine("Happy meme o'clock!");
                    }

                    await guild.DefaultChannel.SendMessageAsync(sb.ToString());
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured during MOC: {0}", e.Message);
            }
        }
    }
}
