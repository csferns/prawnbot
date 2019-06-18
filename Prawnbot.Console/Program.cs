using Microsoft.Extensions.Configuration;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Framework;
using Prawnbot.Core.ServiceLayer;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.ConsoleApp
{
    internal class Program
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
                Console.Title = "Prawnbot.Console";
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Clear();
                Console.OutputEncoding = Encoding.Unicode;

                Program program = new Program();

                AppDomain.CurrentDomain.ProcessExit += program.CurrentDomain_ProcessExit;

                IConfigurationBuilder builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("Configuration\\appsettings.json", optional: false);

                IConfigurationRoot config = builder.Build();

                string token = null;

                if (string.IsNullOrEmpty(ConfigUtility.BotToken) || string.IsNullOrWhiteSpace(ConfigUtility.BotToken))
                {
                    Console.WriteLine("Enter your Access Token: ");
                    token = Console.ReadLine();
                }
                else
                {
                    token = ConfigUtility.BotToken;
                }

                Console.Clear();

                program.workerCancellationTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(async () => 
                {
                    program.workerCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    await program.MainProgram(token);

                }, program.workerCancellationTokenSource.Token);

                program._consoleService = new ConsoleService(program.workerCancellationTokenSource);
                program.CommandListener().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Don't be a dingbat: {e.Message}");
                throw;
            }
        }

        private async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await _botService.DisconnectAsync(false);
        }

        private async Task MainProgram(string token)
        {
            try
            {
                await _botService.ConnectAsync(token, workerCancellationTokenSource);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CommandListener()
        {
            bool result = true;

            while (result && !workerCancellationTokenSource.IsCancellationRequested)
            {
                string command = Console.ReadLine();

                if (command.StartsWith('/'))
                {
                    if (_consoleService.ValidCommand(command).Entity)
                    {
                        Response<bool> response = await _consoleService.HandleConsoleCommand(command);
                        result = response.Entity;
                    }
                    if (string.IsNullOrWhiteSpace(command))
                    {
                        Console.WriteLine("No command entered. Please enter a command.");
                        continue;
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

            Main(new string[] { });
        }
    }
}
