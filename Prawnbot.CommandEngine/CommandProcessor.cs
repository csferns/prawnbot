﻿using Discord;
using Discord.WebSocket;
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
        private readonly ILogging logging;
        private readonly ICoreBL coreBL;
        private readonly IBotBL botBL;

        public CommandProcessor(ILogging logging, ICoreBL coreBL, IBotBL botBL)
        {
            this.logging = logging;
            this.coreBL = coreBL;
            this.botBL = botBL;
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
                    await logging.Log_Info($"Could not find user '{userToGet}'");
                    return;
                }

                string message = command.CommandComponents[1];
                await coreBL.SendDMAsync(user, message);
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
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
                    await logging.Log_Warning($"Could not find guild '{guildName}'");
                    return;
                }

                SocketTextChannel textChannel = coreBL.FindTextChannel(guild, channelName);

                if (textChannel == null)
                {
                    await logging.Log_Warning($"Could not find text channel '{channelName}'");
                    return;
                }

                string message = command.CommandComponents[2];

                await textChannel.SendMessageAsync(message);
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
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

                await logging.Log_Info("Updated successfully");
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
            }
        }

        private void Command_Log()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = ConfigUtility.TextFileDirectory,
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
                await logging.Log_Info($"Nickname changed for {guildName}");
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
            }
        }

        private async Task Command_RemoveIcon(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                await coreBL.ChangeIconAsync();

                await logging.Log_Info("Icon removed.");
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
            }
        }

        private async Task Command_ChangeIcon(Command command)
        {
            if (command.HasCorrectParameterCount)
            {
                Uri.TryCreate(command.CommandComponents[0], UriKind.RelativeOrAbsolute, out Uri imageUri);
                await coreBL.ChangeIconAsync(imageUri);

                await logging.Log_Info("Icon changed.");
            }
            else
            {
                await logging.Log_Info("Invalid number of arguments!");
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

            await logging.Log_Info(helpText.ToString());
        }
    }
}
