using Autofac;
using Autofac.Extensions.DependencyInjection;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Infrastructure;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Application
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (IContainer container = AutofacSetup())
            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                IBotService botService = scope.Resolve<IBotService>();
                ILogging logging = scope.Resolve<ILogging>();
                IConsoleService consoleService = scope.Resolve<IConsoleService>();
                BaseApplication adminApplication = new BaseApplication(botService, logging, consoleService);

                adminApplication.Main(container);
            };
        }

        private static IContainer AutofacSetup()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            containerBuilder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(CoreBL)))
                .Where(assembly => assembly.Name.EndsWith("BL") || assembly.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<Logging>().As<ILogging>().InstancePerLifetimeScope();

            return containerBuilder.Build();
        }
    }

    public class BaseApplication
    {
        private readonly IConsoleService consoleService;
        private readonly IBotService botService;
        private readonly ILogging logging;
        public BaseApplication(IBotService botService, ILogging logging, IConsoleService consoleService)
        {
            this.botService = botService;
            this.logging = logging;
            this.consoleService = consoleService;
        }

        public void Main(IContainer autofacContainer)
        {
            try
            {
                Console.Title = "Prawnbot.Application";
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Clear();
                Console.OutputEncoding = Encoding.Unicode;

                string token = string.IsNullOrEmpty(ConfigUtility.BotToken) || string.IsNullOrWhiteSpace(ConfigUtility.BotToken) 
                    ? Console.ReadLine() 
                    : ConfigUtility.BotToken;

                Console.Clear();

                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                ConnectAsync(token, autofacContainer).GetAwaiter().GetResult();
                CommandListener().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "Main", $"Don't be a dingbat: \n{e.Message}", e)).Wait();
            }
        }

        public async Task ConnectAsync(string token, IContainer autofacContainer)
        {
            try
            {
                await botService.ConnectAsync(token, autofacContainer);
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "Main", $"Don't be a dingbat: \n{e.Message}", e));
            }
        }

        public async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await botService.DisconnectAsync();
            botService.ShutdownQuartz();
        }

        public async Task CommandListener()
        {
            bool result = true;

            while (result)
            {
                string command = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(command))
                {
                    if (command.StartsWith('/'))
                    {
                        if (consoleService.ValidCommand(command).Entity)
                        {
                            Response<bool> response = await consoleService.HandleConsoleCommand(command);
                            result = response.Entity;
                        }
                        else
                        {
                            Console.WriteLine($"'{command.Split(' ')[0]}' is not recognised as a valid command!");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Command needs to start with a '/'");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("No command entered. Please enter a command.");
                    continue;
                }
            }
        }
    }
}
