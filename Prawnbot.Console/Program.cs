using Microsoft.Extensions.Configuration;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.ServiceLayer;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Console
{
    class Program
    {
        private IBotService _botService;
        private IConsoleService _consoleService;
        private CancellationTokenSource workerCancellationTokenSource;
        public Program()
        {
            _botService = new BotService();
            
        }

        public static void Main(string[] args)
        {
            try
            {
                System.Console.Title = "Prawnbot.Console";
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.Clear();
                System.Console.OutputEncoding = Encoding.Unicode;

                Program program = new Program();
                AppDomain.CurrentDomain.ProcessExit += program.CurrentDomain_ProcessExit;

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("Configuration\\appsettings.json", optional: false);

                var config = builder.Build();

                string token = null;

                if (string.IsNullOrEmpty(ConfigUtility.BotToken) || string.IsNullOrWhiteSpace(ConfigUtility.BotToken))
                {
                    System.Console.WriteLine("Enter your Access Token: ");
                    token = System.Console.ReadLine();
                }
                else
                {
                    token = ConfigUtility.BotToken;
                }

                System.Console.Clear();

                program.workerCancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () =>
                {
                    await program.MainProgram(token);
                }, program.workerCancellationTokenSource.Token);

                program._consoleService = new ConsoleService(program.workerCancellationTokenSource);

                program.CommandListener().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                System.Console.Out.WriteLine($"Don't be a dingbat: {e.Message}");
            }
        }

        private async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await _botService.DisconnectAsync(false);
        }

        private async Task MainProgram(string token)
        {
            await _botService.ConnectAsync(token);
        }

        private async Task CommandListener()
        {
            bool result = true;

            while (result)
            {
                string command = System.Console.ReadLine();

                if (command.StartsWith('/'))
                {
                    if (_consoleService.ValidCommand(command).Entity)
                    {
                        var response = await _consoleService.HandleConsoleCommand(command);
                        result = response.Entity;
                    }
                    else
                    {
                        System.Console.WriteLine($"'{command.Split(' ')[0]}' is not recognised as a valid command!");
                        continue;
                    }
                }
                else
                {
                    System.Console.WriteLine("Command needs to start with a '/'");
                    continue;
                }
            }

            Main(new string[] { });
        }
    }
}
