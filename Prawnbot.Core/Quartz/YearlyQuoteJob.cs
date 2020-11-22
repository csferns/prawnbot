﻿using Discord;
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
    public class YearlyQuoteJob : IJob
    {
        private readonly ILogger<YearlyQuoteJob> logger;
        private readonly ICoreService coreService;
        public YearlyQuoteJob(ILogger<YearlyQuoteJob> logger, ICoreService coreService)
        {
            this.logger = logger;
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                ListResponse<IMessage> response = await coreService.GetAllMessagesByTimestampAsync(guildId: 453899130486521859, timestamp: DateTime.Now.AddYears(-1));

                if (response.HasData)
                {
                    HashSet<IMessage> filteredMessages = response.Entities.ToHashSet();

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
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured during YearlyQuote: {0}", e.Message);
            }
        }
    }
}

