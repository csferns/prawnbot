using Autofac;
using Discord;
using Prawnbot.Core.Interfaces;
using Quartz;
using Quartz.Spi;
using System;

namespace Prawnbot.Core.Quartz
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IContainer container;

        /// <summary>
        /// Creates a new instance of the <see cref="QuartzJobFactory"/> class with a dependency injection container to pass into the jobs
        /// </summary>
        /// <param name="container">Dependency injection container</param>
        public QuartzJobFactory(IContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            ILogging logging = container.Resolve<ILogging>();

            try
            {
                Type jobType = bundle.JobDetail.JobType;

                logging.Log_Info($"{jobType.Name} Triggered. Next trigger time: {bundle.NextFireTimeUtc}");

                return (IJob)container.Resolve(jobType);
            }
            catch (Exception e)
            {
                logging.Log_Exception(e, optionalMessage: "An error occured while instantiating a new job");
            }

            return null;
        }

        public void ReturnJob(IJob job)
        {
            IDisposable disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
