using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected static ParallelOptions ParallelOptions { get; } = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        protected static Random Random { get; } = new Random();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (context.FireTimeUtc.ToString("dd/MM") == "11/09")
                {
                    logger.LogInformation("Returning because of the current date");
                    return;
                }

                ListResponse<SocketGuild> guildsResponse = coreService.GetAllGuilds();

                if (guildsResponse.HasData)
                {
                    HashSet<SocketGuild> guilds = guildsResponse.Entities.ToHashSet();

                    DateTime now = DateTime.Now;

                    Parallel.ForEach(guilds, ParallelOptions, async (guild) =>
                    {
                        int mocCount = Random.Next(1, 10);

                        StringBuilder sb = new StringBuilder();

                        sb.AppendFormat("{0:dd/MM/yyyy hh:mm}: {1}", now, mocCount);

                        for (int i = 0; i < mocCount; i++)
                        {
                            sb.Append("\nHappy meme o'clock!");
                        }

                        await guild.DefaultChannel.SendMessageAsync(sb.ToString());
                        logger.LogInformation("Sent MOC message to {0}", guild.Name);
                    });
                }
                else
                {
                    logger.LogWarning("No guilds available to send MOC message to");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured during MOC: {0}", e.Message);
            }
        }
    }
}
