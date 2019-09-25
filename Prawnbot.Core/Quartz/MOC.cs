using Discord;
using Discord.WebSocket;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using Quartz;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class MOC : IJob
    {
        private readonly ILogging logging;
        private readonly ICoreService coreService;
        public MOC(ILogging logging, ICoreService coreService)
        {
            this.logging = logging;
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Quartz", "MOC Triggered."));

                if (DateTime.Now.Date.ToString("dd/MM/yyyy") == $"11/09/{DateTime.Now.Year}")
                {
                    return;
                }

                Bunch<SocketGuild> guilds = coreService.GetAllGuilds().Entities.ToBunch();

                foreach (SocketGuild guild in guilds)
                {
                    Random random = new Random();
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
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Quartz", "An error occured during MOC", e));
            }
        }
    }
}
