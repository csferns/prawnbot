using Prawnbot.Core.ServiceLayer;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Console
{
    class Program
    {
        private IBotService _botService;
        private IConsoleService _consoleService;
        public Program()
        {
            _botService = new BotService();
            _consoleService = new ConsoleService();
        }

        static void Main(string[] args)
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

                System.Console.WriteLine("Enter your Access Token: ");
                string token = System.Console.ReadLine();
                System.Console.Clear();

                Task.Run(async () =>
                {
                    await program.MainProgram(token);
                });

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
            while (true)
            {
                string command = System.Console.ReadLine();

                if (command.StartsWith('/'))
                {
                    if (_consoleService.ValidCommand(command).Entity)
                    {
                        await _consoleService.HandleConsoleCommand(command);
                        continue;
                    }
                    else
                    {
                        System.Console.WriteLine($"'{command}' is not recognised as a valid command!");
                        continue;
                    }
                }
                else
                {
                    System.Console.WriteLine("Command needs to start with a '/'");
                    continue;
                }
            }
        }
    }
}
