using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Log;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Utility.Configuration;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IContainer container = AutofacSetup();

            using ILifetimeScope scope = container.BeginLifetimeScope();
            IBotService botService = scope.Resolve<IBotService>();
            BaseApplication adminApplication = new BaseApplication(botService);

            adminApplication.Main();
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

            containerBuilder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Prawnbot.Core.Modules.Modules)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .InNamespace("Prawnbot.Core.Modules");

            containerBuilder.RegisterType<Logging>().As<ILogging>().InstancePerLifetimeScope();

            return containerBuilder.Build();
        }
    }

    public class BaseApplication
    {
        private readonly IBotService botService;
        public BaseApplication(IBotService botService)
        {
            this.botService = botService;
        }

        public void Main()
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
                ConnectAsync(token).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Don't be a dingbat: {e.Message}");
            }
        }

        public async Task ConnectAsync(string token)
        {
            try
            {
                await botService.ConnectAsync(token);

                await Task.Delay(-1);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await botService.DisconnectAsync(false);
        }
    }
}
