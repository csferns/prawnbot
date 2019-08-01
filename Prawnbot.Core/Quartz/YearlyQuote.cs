using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class YearlyQuote : BaseBL, IJob
    {
        private readonly ILogging logging;
        public YearlyQuote(ILogging logging)
        {
            this.logging = logging;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Quartz", "YearlyQuote Triggered."));

                IEnumerable<IMessage> messages = new List<IMessage>(); //await botBL.GetAllMessages(453899130486521859);
                List<IMessage> filteredMessages = messages.Where(x => x.Timestamp == DateTime.Now.AddYears(-1)).ToList();

                StringBuilder sb = new StringBuilder();

                if (filteredMessages != null)
                {
                    foreach (IMessage message in filteredMessages)
                    {
                        sb.AppendLine(message.Content);
                    }
                }
                else
                {
                    sb.AppendLine("No quotes on this day last year");
                }

                await Client.Guilds.FirstOrDefault(x => x.Id == 266719671171022868).DefaultChannel.SendMessageAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Quartz", "An error occured during YearlyQuote", e));
            }
        }
    }
}

