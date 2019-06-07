using Discord;
using Prawnbot.Core.BusinessLayer;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Prawnbot.Core.Utility
{
    class MOC : BaseBl, IJob
    {
        public MOC()
        {
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await logging.PopulateEventLog(new LogMessage(LogSeverity.Info, "Quartz", "MOC Triggered."));

            Random random = new Random();
            int mocCount = random.Next(6);

            foreach (var guild in _client.Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm")}: {mocCount}");

                for (int i = 0; i < mocCount; i++)
                {
                    await guild.DefaultChannel.SendMessageAsync("Happy meme o'clock!");
                }
            }
        }
    }
}
