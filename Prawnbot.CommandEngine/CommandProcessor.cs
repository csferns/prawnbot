using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Prawnbot.CommandEngine.Interfaces;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.CommandEngine
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ILogger<CommandProcessor> logger;
        private readonly ICoreBL coreBL;
        private readonly IBotBL botBL;
        private readonly IConfigUtility configUtility;

        public CommandProcessor(ILogger<CommandProcessor> logger, ICoreBL coreBL, IBotBL botBL, IConfigUtility configUtility)
        {
            this.logger = logger;
            this.coreBL = coreBL;
            this.botBL = botBL;
            this.configUtility = configUtility;
        }

        public async Task<bool> ProcessCommand(Command command)
        {
            switch (command.ParsedCommand)
            {
                case CommandsEnum.Disconnect:
                    await botBL.DisconnectAsync(shutdown: true);

                    return false;
                case CommandsEnum.SendMessageUser:
                    await Command_SendMessageUser(command);
                    break;
                case CommandsEnum.SendMessageGuild:
                    await Command_SendMessageGuild(command);
                    break;
                case CommandsEnum.RichPresence:
                    await Command_RichPresence(command);
                    break;
                case CommandsEnum.Help:
                    await Command_Help();
                    break;
                case CommandsEnum.Log:
                    Command_Log();
                    break;
                case CommandsEnum.Nickname:
                    await Command_Nickname(command);
                    break;
                case CommandsEnum.ChangeIcon:
                    await Command_ChangeIcon(command);
                    break;
                case CommandsEnum.RemoveIcon:
                    await Command_RemoveIcon(command);
                    break;
                default:
                    break;
            }

            return true;
        }

        private async Task Command_SendMessageUser(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                string userToGet = command.CommandComponents[0];

                SocketGuildUser user = coreBL.GetUser(userToGet);

                if (user == null)
                {
                    logger.LogWarning("Could not find user '{0}'", userToGet);
                    return;
                }

                string message = command.CommandComponents[1];
                await coreBL.SendDMAsync(user, message);
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private async Task Command_SendMessageGuild(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                string guildName = command.CommandComponents[0];
                string channelName = command.CommandComponents[1];

                SocketGuild guild = coreBL.GetGuild(guildName);

                if (guild == null)
                {
                    logger.LogWarning("Could not find guild '{0}'", guildName);
                    return;
                }

                SocketTextChannel textChannel = coreBL.FindTextChannel(guild, channelName);

                if (textChannel == null)
                {
                    logger.LogWarning("Could not find text channel '{0}'", channelName);
                    return;
                }

                string message = command.CommandComponents[2];

                await textChannel.SendMessageAsync(message);
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private async Task Command_RichPresence(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                string activity = command.CommandComponents[0];

                string name = command.CommandComponents[1];

                string streamUrl = command.TotalParameterCount == 3
                    ? command.CommandComponents[2]
                    : null;

                Enum.TryParse<ActivityType>(activity, ignoreCase: true, out ActivityType activityType);

                await coreBL.UpdateRichPresenceAsync(name, activityType, streamUrl);

                logger.LogInformation("Updated successfully");
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private void Command_Log()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = configUtility.TextFileDirectory,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        private async Task Command_Nickname(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                string guildName = command.CommandComponents[0];
                string nickname = command.CommandComponents[1];

                await coreBL.ChangeNicknameAsync(guildName, nickname);
                logger.LogInformation("Nickname changed for {0}", guildName);
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private async Task Command_RemoveIcon(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                await coreBL.ChangeIconAsync();

                logger.LogInformation("Icon removed.");
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private async Task Command_ChangeIcon(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                Uri.TryCreate(command.CommandComponents[0], UriKind.RelativeOrAbsolute, out Uri imageUri);
                await coreBL.ChangeIconAsync(imageUri);

                    logger.LogInformation("Icon changed.");
            }
            else
            {
                logger.LogWarning("Invalid number of arguments!");
            }
        }

        private async Task Command_Help()
        {
            Type commandsEnum = typeof(CommandsEnum);

            IEnumerable<FieldInfo> fields = commandsEnum.GetFields().Where(x => x.FieldType == typeof(CommandsEnum));
            IEnumerable<CommandAttribute> attributes = fields.Select(x => (CommandAttribute)x.GetCustomAttributes(typeof(CommandAttribute), false).First());

            StringBuilder helpText = new StringBuilder("\nAvailable commands:\n");

            foreach (CommandAttribute item in attributes)
            {
                helpText.Append($"\t{item.CommandText}: {item.Description}");
                helpText.Append((item.Parameters?.Any() ?? false) ? $" Required: ({string.Join(", ", item.Parameters)})" : null);
                helpText.AppendLine((item.OptionalParameters?.Any() ?? false) ? $" Optional: ({string.Join(", ", item.OptionalParameters)})" : null);
            }

            logger.LogInformation(helpText.ToString());
        }
    }
}
