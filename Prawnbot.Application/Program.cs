using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Common.Configuration;
using Prawnbot.Core;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Log;
using Prawnbot.Data;
using Prawnbot.Data.Interfaces;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Application 
{
    /// <summary>
    /// Console Application entry point
    /// </summary>
    public class Program
    {
        protected IBotService botService;

        public Program()
        {

        }

        public static async Task Main(string[] args)
        {
            Program program = new Program();
            ILogging logging = null;

            try
            {
                // Get the Dependency Injection container and use it to make a new instance
                // of the non static BaseApplication class so work can be done
                using (IContainer container = Services.DependencyInjectionSetup())
                using (ILifetimeScope scope = container.BeginLifetimeScope())
                {
                    IBotService botService = scope.Resolve<IBotService>();
                    program.botService = botService;

                    logging = scope.Resolve<ILogging>();

                    ICommandEngine commandEngine = scope.Resolve<ICommandEngine>();

                    IConfigUtility configUtility = scope.Resolve<IConfigUtility>();

                    program.SetupEnvironmentConfig(configUtility);

                    string token = string.IsNullOrEmpty(configUtility.BotToken)
                        ? Console.ReadLine() 
                        : configUtility.BotToken;

                    Console.Clear();

                    await botService.ConnectAsync(token, container);
                    await commandEngine.BeginListen(() => Console.ReadLine()); 

                    Thread.Sleep(Timeout.Infinite);
                };
            }
            catch (Exception e)
            {
                logging?.Log_Exception(e);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Event listener to watch for the console being closed and dispose of the Discord Client correctly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await botService.DisconnectAsync(shutdown: true).ConfigureAwait(false);
        }

        private void SetupEnvironmentConfig(IConfigUtility configUtility)
        {
            Console.Title = configUtility.ConsoleTitle;
            Console.BackgroundColor = configUtility.ConsoleBackground;
            Console.ForegroundColor = configUtility.ConsoleForeground;
            Console.OutputEncoding = configUtility.ConsoleEncoding;

            Console.Clear();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }
    }
}
