using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Common.Configuration;
using Prawnbot.Core;
using Prawnbot.Core.Interfaces;
using System;
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
        protected IConfigUtility configUtility;

        public Program()
        {

        }

        public static async Task Main(string[] args)
        {
            Program program = new Program();

            IServiceProvider provider = Services.DependencyInjectionSetup();

            ILogger<Program> logger = provider.GetService<ILogger<Program>>();

            try
            {
                IBotService botService = provider.GetService<IBotService>();
                program.botService = botService;

                IConfigUtility configUtility = provider.GetService<IConfigUtility>();
                program.configUtility = configUtility;

                ICommandEngine commandEngine = provider.GetService<ICommandEngine>();

                program.SetupEnvironmentConfig();

                string token = string.IsNullOrEmpty(configUtility.BotToken)
                    ? Console.ReadLine() 
                    : configUtility.BotToken;

                Console.Clear();

                await botService.ConnectAsync(token, provider);
                await commandEngine.BeginListen(() => Console.ReadLine()); 

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured: {0}", e.Message);

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

        private void SetupEnvironmentConfig()
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
