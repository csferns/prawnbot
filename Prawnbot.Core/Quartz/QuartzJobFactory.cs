using Microsoft.Extensions.Logging;
using Prawnbot.Core.Interfaces;
using Quartz;
using Quartz.Spi;
using System;

namespace Prawnbot.Core.Quartz
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider provider;
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new instance of the <see cref="QuartzJobFactory"/> class with a dependency injection container to pass into the jobs
        /// </summary>
        /// <param name="container">Dependency injection container</param>
        public QuartzJobFactory(ILogger logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                Type jobType = bundle.JobDetail.JobType;

                logger.LogInformation("{0} triggered. Next trigger time: {1}", jobType.Name, bundle.NextFireTimeUtc);

                return (IJob)provider.GetService(jobType);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while instantiating a new job: {0}", e.Message);
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
