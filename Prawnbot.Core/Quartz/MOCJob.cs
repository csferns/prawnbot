using Discord.WebSocket;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Logging;
using Quartz;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class MOCJob : IJob
    {
        private readonly ILogging logging;
        private readonly ICoreService coreService;
        public MOCJob(ILogging logging, ICoreService coreService)
        {
            this.logging = logging;
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

                IBunch<SocketGuild> guilds = coreService.GetAllGuilds().Entities.ToBunch();

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
                logging.Log_Exception(e, optionalMessage: "An error occured during MOC");
            }
        }
    }
}
