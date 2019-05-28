using Microsoft.Extensions.Configuration;
using Prawnbot.Core.Bot;
using System;
using System.IO;
using System.Text;
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
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.Clear();
            System.Console.OutputEncoding = Encoding.Unicode;

            Program program = new Program();
            AppDomain.CurrentDomain.ProcessExit += program.CurrentDomain_ProcessExit;

            program.MainProgram().GetAwaiter().GetResult();
        }

        private async void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            await _botService.DisconnectAsync(false);
        }

        private async Task MainProgram()
        {
            System.Console.WriteLine("Enter your Access Token: ");
            string token = System.Console.ReadLine();
            System.Console.Clear();

            await _botService.ConnectAsync(token);

            await Task.Delay(-1);
        }
    }
}
