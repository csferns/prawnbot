using Discord.WebSocket;
using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using Quartz;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public class CalendarJob : IJob
    {
        private readonly IAPIService apiService;
        private readonly ICoreService coreService;

        public CalendarJob(IAPIService apiService, ICoreService coreService)
        {
            this.apiService = apiService;
            this.coreService = coreService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ListResponse<Event> ukHolidays = await apiService.GetCalendarEntries("en.uk#holiday@group.v.calendar.google.com");
            ListResponse<Event> moonPhases = await apiService.GetCalendarEntries("ht3jlfaac5lfd6263ulfh4tql8@group.calendar.google.com");

            ListResponse<SocketGuild> guildsResponse = coreService.GetAllGuilds();

            if (guildsResponse.HasData)
            {
                foreach (SocketGuild guild in guildsResponse.Entities)
                {
                    await guild.DefaultChannel.SendMessageAsync($"UK Holidays: {string.Join(',', ukHolidays.Entities)}");
                    await guild.DefaultChannel.SendMessageAsync($"Moon Phase: {string.Join(',', moonPhases.Entities)}");
                }
            }
        }
    }
}
