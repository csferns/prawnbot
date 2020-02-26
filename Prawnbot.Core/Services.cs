using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Log;
using Prawnbot.Data;
using Prawnbot.Data.Interfaces;
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
                                || assembly.Name.EndsWith("Repository", StringComparison.InvariantCultureIgnoreCase)
                                || assembly.Name.EndsWith("Job", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(Assembly.Load("Prawnbot.CommandEngine"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // The logging dependency won't be picked up in the above condition so set it here
            containerBuilder.RegisterType<Logging>().As<ILogging>().InstancePerLifetimeScope();

            // TODO: This really shouldn't be InstancePerLifetimeScope, they should be InstancePerRequest so the call to the database gets disposed when it is finished.
            containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            // return a built collection of services
            return containerBuilder.Build();
        }
    }
}
