using Prawnbot.Core.Bot;
using System;
using System.Threading.Tasks;

namespace Prawnbot.Console
{
    class Program
    {
        private IBotService _botService;
        public Program()
        {
            _botService = new BotService();
        }

        static void Main(string[] args)
        {
            System.Console.Title = "Prawnbot.Console";
            System.Console.BackgroundColor = ConsoleColor.White;
            System.Console.ForegroundColor = ConsoleColor.DarkMagenta;
            System.Console.Clear();

            Program program = new Program();
            program.MainProgram().Wait();
        }

        private async Task MainProgram()
        {
            System.Console.WriteLine("Enter your Access Token: ");
            string token = System.Console.ReadLine();

            await _botService.ConnectAsync(token);

            System.Console.WriteLine("Connected.");

            await Task.Delay(-1);
        }
    }
}
