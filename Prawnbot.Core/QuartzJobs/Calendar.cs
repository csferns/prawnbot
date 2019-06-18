using Discord.WebSocket;
using Google.Apis.Calendar.v3.Data;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Framework;
using Quartz;
using System.Threading.Tasks;

namespace Prawnbot.Core.QuartzJobs
{
    public class Calendar : BaseBl, IJob
    {
        public Calendar()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            ListResponse<Event> ukHolidays = await _apiService.GetCalendarEntries("en.uk#holiday@group.v.calendar.google.com");
            ListResponse<Event> moonPhases = await _apiService.GetCalendarEntries("ht3jlfaac5lfd6263ulfh4tql8@group.calendar.google.com");

            foreach (SocketGuild guild in _client.Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync($"UK Holidays: {string.Join(',', ukHolidays.Entities)}");
                await guild.DefaultChannel.SendMessageAsync($"Moon Phase: {string.Join(',', moonPhases.Entities)}");
            }
        }
    }
}
