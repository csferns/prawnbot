using Discord;
using Discord.WebSocket;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using Quartz;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class YearlyQuoteJob : IJob
    {
        private readonly ILogging logging;
        private readonly ICoreService coreService;
        public YearlyQuoteJob(ILogging logging, ICoreService coreService)
        {
            this.logging = logging;
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                ListResponse<IMessage> response = await coreService.GetAllMessagesByTimestampAsync(guildId: 453899130486521859, timestamp: DateTime.Now.AddYears(-1));
                Bunch<IMessage> filteredMessages = response.Entities.ToBunch();

                StringBuilder sb = new StringBuilder();

                if (filteredMessages != null || filteredMessages.Any())
                {
                    foreach (IMessage message in filteredMessages)
                    {
                        string content = message.Content;

                        if (content.Length > 250)
                        {
                            content = content.Substring(0, 247);
                            content += "...";
                        }
                        
                        sb.AppendLine(content);
                    }
                }
                else
                {
                    sb.AppendLine("No quotes on this day last year");
                }

                SocketTextChannel defaultChannel = coreService.GetGuildById(guildId: 453899130486521859).Entity.DefaultChannel;
                await defaultChannel.SendMessageAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e, optionalMessage: "An error occured during YearlyQuote");
            }
        }
    }
}

