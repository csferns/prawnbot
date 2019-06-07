using Prawnbot.Core.BusinessLayer;
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
            var ukHolidays = await _apiService.GetCalendarEntries("en.uk#holiday@group.v.calendar.google.com");
            var moonPhases = await _apiService.GetCalendarEntries("ht3jlfaac5lfd6263ulfh4tql8@group.calendar.google.com");

            foreach (var guild in _client.Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync($"UK Holidays: {string.Join(',', ukHolidays.Entities)}");
                await guild.DefaultChannel.SendMessageAsync($"Moon Phase: {string.Join(',', moonPhases.Entities)}");
            }
        }
    }
}
