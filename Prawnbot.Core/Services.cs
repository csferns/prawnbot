using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Log;
using Prawnbot.Data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prawnbot.Core
{
    public static class Services
    {
        /// <summary>
        /// Creates a Dependency Injection container with relevant services in
        /// </summary>
        /// <returns>IContainer</returns>
        public static IServiceProvider DependencyInjectionSetup(IServiceCollection originalCollection = null)
        {
            // Microsoft DI
            IServiceCollection serviceCollection = originalCollection ?? new ServiceCollection();
            serviceCollection.AddLogging((logging) => 
            {
                logging.ClearProviders();

                const string fileName = "..\\EventLogs.txt";
                logging.AddProvider(new CustomLoggerProvider(new CustomLoggerConfiguration() { FileName = fileName, LogLevel = LogLevel.Information, ConsoleColour = ConsoleColor.Cyan }));
                logging.AddProvider(new CustomLoggerProvider(new CustomLoggerConfiguration() { FileName = fileName, LogLevel = LogLevel.Warning, ConsoleColour = ConsoleColor.Yellow }));
                logging.AddProvider(new CustomLoggerProvider(new CustomLoggerConfiguration() { FileName = fileName, LogLevel = LogLevel.Error, ConsoleColour = ConsoleColor.Red }));
                logging.AddProvider(new CustomLoggerProvider(new CustomLoggerConfiguration() { FileName = fileName, LogLevel = LogLevel.Critical, ConsoleColour = ConsoleColor.DarkRed }));
                logging.AddProvider(new CustomLoggerProvider(new CustomLoggerConfiguration() { FileName = fileName, LogLevel = LogLevel.Debug, ConsoleColour = ConsoleColor.White }));
            });

            // Set up database connection
            serviceCollection.AddDbContext<BotDatabaseContext>();

            Assembly coreAssembly = Assembly.Load("Prawnbot.Core");
            IEnumerable<Type> relevantServices = coreAssembly.GetTypes()
                                                             .Where(x => x.Name.EndsWith("BL", StringComparison.InvariantCultureIgnoreCase)
                                                                      || x.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase));

            relevantServices = relevantServices.Concat(Assembly.Load("Prawnbot.CommandEngine").GetTypes());
            relevantServices = relevantServices.Concat(Assembly.Load("Prawnbot.Common").GetTypes());

            foreach (Type service in relevantServices.Where(x => !x.IsInterface && x.GetInterfaces().Any() && !x.IsEnum))
            {
                Type serviceInterface = service.GetInterface("I" + service.Name);

                if (serviceInterface != null)
                {
                    serviceCollection.AddScoped(serviceInterface, service);
                }
            }

            IEnumerable<Type> jobs = coreAssembly.GetTypes().Where(x => x.Name.EndsWith("Job", StringComparison.InvariantCultureIgnoreCase));
            Type jobType = typeof(IJob);

            foreach (Type job in jobs)
            {
                serviceCollection.AddScoped(jobType, job);
            }

            // return a built collection of services
            return serviceCollection.BuildServiceProvider();
        }
    }
}
