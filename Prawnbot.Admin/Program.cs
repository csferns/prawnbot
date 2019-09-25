using Prawnbot.Core.ServiceLayer;
using Prawnbot.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Prawnbot.Admin
{
    class Program
    {
        static void Main(string[] args)
        {
            //AdminApplication adminApplication = new AdminApplication();
            //adminApplication.CommandListener().GetAwaiter().GetResult();
        }
    }

    public class AdminApplication
    {
        private readonly IConsoleService consoleService;
        public AdminApplication(IConsoleService consoleService)
        {
            this.consoleService = consoleService;
        }

        public async Task CommandListener()
        {
            bool result = true;

            while (result)
            {
                string command = Console.ReadLine();

                if (command.StartsWith('/'))
                {
                    if (consoleService.ValidCommand(command).Entity)
                    {
                        Response<bool> response = await consoleService.HandleConsoleCommand(command);
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
        }
    }
}
