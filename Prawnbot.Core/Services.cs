using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Data;
using Prawnbot.Data.Interfaces;
using Prawnbot.Logging;
using System;
using System.Reflection;

namespace Prawnbot.Core
{
    public class Services
    {
        private IContainer Container;

        public Services()
        {

        }

        /// <summary>
        /// Creates a Dependency Injection container with relevant services in
        /// </summary>
        /// <returns>IContainer</returns>
        private IContainer DependencyInjectionSetup(IServiceCollection originalCollection = null)
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
            containerBuilder.RegisterAssemblyTypes(Assembly.Load("Prawnbot.Core"), Assembly.Load("Prawnbot.Core.Database"), Assembly.Load("Prawnbot.FileHandling"))
                .Where(assembly => assembly.Name.EndsWith("BL", StringComparison.InvariantCultureIgnoreCase) 
                                || assembly.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase) 
                                || assembly.Name.EndsWith("Repository", StringComparison.InvariantCultureIgnoreCase)
                                || assembly.Name.EndsWith("Job", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(Assembly.Load("Prawnbot.Logging"), Assembly.Load("Prawnbot.CommandEngine"), Assembly.Load("Prawnbot.Common"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            // TODO: This really shouldn't be InstancePerLifetimeScope, they should be InstancePerRequest so the call to the database gets disposed when it is finished.
            containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            Container = containerBuilder.Build();

            // return a built collection of services
            return Container;
        }

        public IContainer Get()
        {
            if (Container != null)
            {
                return Container;
            }
            else 
            {
                return DependencyInjectionSetup();
            }
        }

        public IContainer Get(IServiceCollection originalCollection)
        {
            if (Container != null)
            {
                return Container;
            }
            else
            {
                return DependencyInjectionSetup(originalCollection);
            }
        }
    }
}
