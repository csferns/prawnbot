using Autofac;
using Autofac.Extensions.DependencyInjection;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Exceptions;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Utility.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (IContainer container = AutofacSetup())
            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                IBotService botService = scope.Resolve<IBotService>();
                ILogging logging = scope.Resolve<ILogging>();
                BaseApplication adminApplication = new BaseApplication(botService, logging);

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
        private readonly IBotService botService;
        private readonly ILogging logging;
        public BaseApplication(IBotService botService, ILogging logging)
        {
            this.botService = botService;
            this.logging = logging;
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
                Process currentProcess = Process.GetCurrentProcess();
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "ConnectAsync", $"Process {currentProcess.ProcessName} ({currentProcess.Id}) started on {Environment.MachineName} "));

                await botService.ConnectAsync(token, autofacContainer);

                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Debug, "ConnectAsync", $"Memory used before collection: {GC.GetTotalMemory(false)}"));
                GC.Collect();
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Debug, "ConnectAsync", $"Memory used after collection: {GC.GetTotalMemory(true)}"));

                await Task.Delay(-1);
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
    }
}
