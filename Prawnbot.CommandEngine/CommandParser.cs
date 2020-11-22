using Microsoft.Extensions.Logging;
using Prawnbot.CommandEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class CommandParser : ICommandParser
    {
        private readonly static Regex CommandRegex = new Regex(@"(""(.+?)"")|([^\s""]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly ILogger<CommandParser> logger;

        public CommandParser(ILogger<CommandParser> logger)
        {
            this.logger = logger;
        }

        private async Task<bool> EnsureValidCommand(Command command)
        {
            if (string.IsNullOrEmpty(command.CommandText))
            {
                logger.LogWarning("No command entered. Please enter a command.");
                return false;
            }

            if (!command.CommandText.StartsWith('/'))
            {
                logger.LogWarning("Command needs to start with a '/'");
                return false;
            }

            List<string> split = CommandRegex.Split(command.CommandText).ToList();
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
                logger.LogWarning("'{0}' is not recognised as a valid command!", firstWord);
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

            logger.LogInformation("Command recieved through console: '{0}'", commandText);

            bool valid = await EnsureValidCommand(command);

            command.Valid = valid;

            return command;
        }
    }
}
