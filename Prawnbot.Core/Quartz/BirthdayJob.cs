using System;
using Prawnbot.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Quartz;

namespace Prawnbot.Core.Quartz
{
    public class BirthdayJob : IJob
    {
        private readonly ICoreService coreService;

        public BirthdayJob(ICoreService coreService)
        {
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ListResponse<SocketGuild> guildsResponse = coreService.GetAllGuilds();

            if (guildsResponse.HasData)
            {
                Bunch<SocketGuild> guilds = guildsResponse.Entities.ToBunch();

                foreach (SocketGuild guild in guilds)
                {
                    ListResponse<SocketGuildUser> usersResponse = coreService.GetUsersByGuildId(guild.Id);

                    if (usersResponse.HasData)
                    {
                        IEnumerable<SocketGuildUser> birthdayUsers = usersResponse.Entities.Where(x => !x.IsBot && x.JoinedAt.Value.Date == DateTime.Today);

                        if (birthdayUsers.Any())
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (SocketGuildUser user in birthdayUsers)
                            {
                                TimeSpan activeTimeSpan = DateTime.Today - user.JoinedAt.Value.Date;

                                sb.AppendFormat("{0} joined the server on {1}, meaning that you have been on the server {2} years today, happy anniversary!\n", user.Mention, user.JoinedAt, (activeTimeSpan.TotalDays / 365));
                            }

                            await guild.DefaultChannel.SendMessageAsync(sb.ToString());
                        }
                    } 
                }
            }
            
        }
    }
}
