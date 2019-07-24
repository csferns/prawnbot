using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Prawnbot.Core.Quartz
{
    public static class QuartzSetup
    {
        public static async Task Setup() 
        {
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            IJobDetail mocJob = JobBuilder.Create<MOC>()
                .WithIdentity("MOC")
                .Build();

            ITrigger mocTrigger = TriggerBuilder.Create()
                .WithIdentity("MOCTrigger")
                .StartAt(DateBuilder.DateOf(09, 11, 00))
                .WithSimpleSchedule(x => x
                                        .WithIntervalInHours(12)
                                        .RepeatForever())
                .ForJob("MOC")
                .Build();

            //$"Job {typeof(MOC)} triggered"

            await scheduler.ScheduleJob(mocJob, mocTrigger);

            IJobDetail yearlyQuoteJob = JobBuilder.Create<YearlyQuote>()
                .WithIdentity("YearlyQuote")
                .Build();

            ITrigger yearlyQuoteTrigger = TriggerBuilder.Create()
                .WithIdentity("YearlyQuoteTrigger")
                .StartAt(DateBuilder.DateOf(08, 00, 00))
                .WithSimpleSchedule(x => x
                                        .WithIntervalInHours(24)
                                        .RepeatForever())
                .ForJob("YearlyQuote")
                .Build();

            await scheduler.ScheduleJob(yearlyQuoteJob, yearlyQuoteTrigger);
        }
    }
}
