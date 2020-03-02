using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class CommandParser : ICommandParser
    {
        private readonly static Regex CommandRegex = new Regex(@"(""(.+?)"")|([^\s""]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly ILogging logging;

        public CommandParser(ILogging logging)
        {
            this.logging = logging;
        }

        private async Task<bool> EnsureValidCommand(Command command)
        {
            if (string.IsNullOrEmpty(command.CommandText))
            {
                await logging.Log_Info("No command entered. Please enter a command.");
                return false;
            }

            if (!command.CommandText.StartsWith('/'))
            {
                await logging.Log_Info("Command needs to start with a '/'");
                return false;
            }

            Bunch<string> split = CommandRegex.Split(command.CommandText).ToBunch();
            split.RemoveAll(x => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x));

            if (!split.Any())
            {
                return false;
            }

            string firstWord = split[0].Replace("/", "");

            bool doesParse = Enum.TryParse<CommandsEnum>(firstWord, ignoreCase: true, out CommandsEnum commandsEnum);

            if (doesParse)
            {
                command.ParsedCommand = commandsEnum;

                split.RemoveAt(0);
                command.CommandComponents = split;

                GetCommandInformation(command);

                return true;
            }
            else
            {
                await logging.Log_Info($"'{firstWord}' is not recognised as a valid command!");
                return false;
            }
        }

        private void GetCommandInformation(Command command)
        {
            Type commandsEnum = typeof(CommandsEnum);

            FieldInfo field = commandsEnum.GetField(command.ParsedCommand.ToString());
            object[] attributes = field.GetCustomAttributes(typeof(CommandAttribute), false);

            if (attributes.Any())
            {
                CommandAttribute attribute = (CommandAttribute)attributes.First();

                command.RequiredParameterCount = attribute.Parameters?.Count() ?? 0;
                command.OptionalParameterCount = attribute.OptionalParameters?.Count();
            }
        }

        public async Task<Command> ParseCommand(string commandText)
        {
            Command command = new Command()
            {
                CommandText = commandText
            };

            await logging.Log_Info($"Command recieved through console: '{commandText}'", updateConsole: false);

            bool valid = await EnsureValidCommand(command);

            command.Valid = valid;

            return command;
        }
    }
}
