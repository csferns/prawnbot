using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Log;
using Prawnbot.Data;
using System;
using System.Reflection;

namespace Prawnbot.Core
{
    public static class Services
    {
        /// <summary>
        /// Creates a Dependency Injection container with relevant services in
        /// </summary>
        /// <returns>IContainer</returns>
        public static IContainer DependencyInjectionSetup(IServiceCollection originalCollection = null)
        {
            // Microsoft DI
            IServiceCollection serviceCollection = originalCollection ?? new ServiceCollection();
            serviceCollection.AddLogging();

            // Set up database connection
            serviceCollection.AddDbContext<BotDatabaseContext>();

            // Autofac DI
            // Create a new container and populate the Microsoft DI components
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            // Add dependencies based on Assembly and name
            containerBuilder.RegisterAssemblyTypes(Assembly.Load("Prawnbot.Core"))
                .Where(assembly => assembly.Name.EndsWith("BL", StringComparison.InvariantCultureIgnoreCase) 
                                || assembly.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase) 
                                || assembly.Name.EndsWith("Job", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(Assembly.Load("Prawnbot.CommandEngine"), Assembly.Load("Prawnbot.Common"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // The logging dependency won't be picked up in the above condition so set it here
            containerBuilder.RegisterType<Logging>().As<ILogging>().InstancePerLifetimeScope();

            // return a built collection of services
            return containerBuilder.Build();
        }
    }
}
