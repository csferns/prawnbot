using Prawnbot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IConsoleBl
    {
        bool ValidCommand(string command);
        Task HandleConsoleCommand(string command);
        void ConsoleCommands(string command);
    }

    public class ConsoleBl : BaseBl, IConsoleBl
    {
        private readonly List<CommandsEnum> availableCommands = new List<CommandsEnum>()
        {
            CommandsEnum.disconnect,
            CommandsEnum.sendmessage,
            CommandsEnum.richpresence
        };

        public bool ValidCommand(string command)
        {
            command = command.Remove(0, 1);
            string[] commandComponents = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Enum.TryParse(commandComponents[0].ToLower(), out CommandsEnum commands);

            return availableCommands.Contains(commands);
        }

        public async Task HandleConsoleCommand(string command)
        {
            command = command.Remove(0, 1);
            string[] commandComponents = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Enum.TryParse(commandComponents[0].ToLower(), out CommandsEnum commands);

            switch (commands)
            {
                case CommandsEnum.disconnect:
                    await _botBl.DisconnectAsync(true);
                    break;
                case CommandsEnum.sendmessage:
                    break;
                case CommandsEnum.richpresence:
                    break;
                default:
                    break;
            }
        }

        public void ConsoleCommands(string command)
        {

        }
    }
}
