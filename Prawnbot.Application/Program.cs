using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Log;
using Prawnbot.Data;
using Prawnbot.Data.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Prawnbot.Application
{
    /// <summary>
    /// Console Application entry point
    /// </summary>
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Get the Dependency Injection container and use it to make a new instance
            // of the non static BaseApplication class so work can be done
            using (IContainer container = DependencyInjectionSetup())
            {
                BaseApplication adminApplication = new BaseApplication(container);

                Task.WaitAll(Task.Run(async () => await adminApplication.Main()));
            };
        }

        /// <summary>
        /// Creates a Dependency Injection container with relevant services in
        /// </summary>
        /// <returns>IContainer</returns>
        private static IContainer DependencyInjectionSetup()
        {
            // Microsoft DI
            IServiceCollection serviceCollection = new ServiceCollection();
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

    /// <summary>
    /// Main Console Application class
    /// </summary>
    public class BaseApplication
    {
        private readonly IContainer container;
        private readonly ICommandEngine commandEngine;
        private readonly IBotService botService;
        private readonly ILogging logging;

        /// <summary>
        /// Creates a new instance of the <see cref="BaseApplication"/> class with a container being passed in to populate the required services
        /// </summary>
        /// <param name="container"></param>
        public BaseApplication(IContainer container)
        {
            this.container = container;

            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                this.botService = scope.Resolve<IBotService>();
                this.logging = scope.Resolve<ILogging>();
                this.commandEngine = scope.Resolve<ICommandEngine>();
            }
        }

        /// <summary>
        /// Main Console Application work
        /// </summary>
        public async Task Main()
        {
            try
            {
                Console.Title = ConfigUtility.ConsoleTitle;
                Console.BackgroundColor = ConfigUtility.ConsoleBackground;
                Console.ForegroundColor = ConfigUtility.ConsoleForeground;
                Console.OutputEncoding = ConfigUtility.ConsoleEncoding;

                Console.Clear();

                string token = string.IsNullOrEmpty(ConfigUtility.BotToken)
                    ? Console.ReadLine() 
                    : ConfigUtility.BotToken;

                Console.Clear();

                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                await ConnectAsync(token, container);

                await commandEngine.BeginListen(() => Console.ReadLine());
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Connect the bot
        /// </summary>
        /// <param name="token"></param>
        /// <param name="autofacContainer"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string token, IContainer autofacContainer)
        {
            try
            {
                await botService.ConnectAsync(token, autofacContainer);
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
            }
        }

        /// <summary>
        /// Event listener to watch for the console being closed and dispose of the Discord Client correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await botService.DisconnectAsync();
        }
    }
}
