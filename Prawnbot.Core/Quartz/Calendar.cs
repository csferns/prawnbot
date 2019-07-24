using Discord.WebSocket;
using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Infrastructure;
using Quartz;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class Calendar : BaseBL, IJob
    {
        private readonly IAPIService apiService;
        public Calendar(IAPIService apiService)
        {
            this.apiService = apiService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ListResponse<Event> ukHolidays = await apiService.GetCalendarEntries("en.uk#holiday@group.v.calendar.google.com");
            ListResponse<Event> moonPhases = await apiService.GetCalendarEntries("ht3jlfaac5lfd6263ulfh4tql8@group.calendar.google.com");

            foreach (SocketGuild guild in Client.Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync($"UK Holidays: {string.Join(',', ukHolidays.Entities)}");
                await guild.DefaultChannel.SendMessageAsync($"Moon Phase: {string.Join(',', moonPhases.Entities)}");
            }
        }
    }
}
