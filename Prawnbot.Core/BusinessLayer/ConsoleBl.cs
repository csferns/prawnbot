using Discord;
using Prawnbot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IConsoleBl
    {
        bool ValidCommand(string command);
        Task<bool> HandleConsoleCommand(string command);
    }

    public class ConsoleBl : BaseBl, IConsoleBl
    {
        private CancellationTokenSource workerCancellationTokenSource;

        public ConsoleBl(CancellationTokenSource workerCancellationTokenSource)
        {
            this.workerCancellationTokenSource = workerCancellationTokenSource;
        }

        public bool ValidCommand(string command)
        {
            try
            {
                command = command.Remove(0, 1);
                string[] commandComponents = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var enumParse = (CommandsEnum)Enum.Parse(typeof(CommandsEnum), commandComponents[0].ToLower());

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public async Task<bool> HandleConsoleCommand(string command)
        {
            command = command.Remove(0, 1);
            List<string> commandComponents = command.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            var commands = (CommandsEnum)Enum.Parse(typeof(CommandsEnum), commandComponents[0].ToLower());

            switch (commands)
            {
                case CommandsEnum.help:
                    Dictionary<string, string> commandsDictionary = new Dictionary<string, string>()
                    {
                        { "disconnect", "(No additional parameters)" },
                        { "sendmessage {dm}", "{username}" },
                        { "sendmessage {guild}", "{guild name} {guild channel name}" },
                        { "richpresence", "{activitytype} {name} {(Optional) stream url}" }
                    };

                    commandComponents.Remove("help");
                    if (commandComponents.Count() > 0)
                    {
                        var givenCommand = commandsDictionary.Where(x => x.Key.Contains(commandComponents[0]));

                        if (givenCommand.Count() == 0)
                        {
                            await Console.Out.WriteLineAsync($"Could not find command '{commandComponents[0]}'");
                        }

                        await Console.Out.WriteLineAsync();

                        foreach (var givenCommandItem in givenCommand)
                        {
                            await Console.Out.WriteLineAsync($"/{givenCommandItem.Key} {givenCommandItem.Value}");
                        }

                    }
                    else
                    {
                        await Console.Out.WriteLineAsync();

                        foreach (var commandItem in commandsDictionary)
                        {
                            await Console.Out.WriteLineAsync($"/{commandItem.Key} {commandItem.Value}");
                        }
                    }

                    return true;
                case CommandsEnum.disconnect:
                    workerCancellationTokenSource.Cancel();
                    await _botBl.DisconnectAsync(true);

                    Console.Clear();

                    return false;
                case CommandsEnum.sendmessage:
                    commandComponents.Remove("sendmessage");
                    if (commandComponents.Count() == 2 || commandComponents.Count() == 3)
                    {
                        switch (commandComponents[0].ToLower())
                        {
                            case "guild":
                                {
                                    var guild = _botBl.GetGuild(commandComponents[1]);

                                    if (guild == null)
                                    {
                                        await Console.Out.WriteLineAsync($"Could not find guild '{commandComponents[1]}'");
                                        return true;
                                    }

                                    var textChannel = _botBl.FindTextChannel(guild, commandComponents[2]);

                                    if (textChannel == null)
                                    {
                                        await Console.Out.WriteLineAsync($"Could not find text channel '{commandComponents[2]}'");
                                        return true;
                                    }

                                    await Console.Out.WriteLineAsync("Please enter the message you want to send...");
                                    var message = Console.ReadLine();

                                    await textChannel.SendMessageAsync(message);
                                    return true;
                                }
                            case "dm":
                                {
                                    var user = _botBl.GetUser(commandComponents[1]);

                                    if (user == null)
                                    {
                                        await Console.Out.WriteLineAsync($"Could not find user '{commandComponents[1]}'");
                                        return true;
                                    }

                                    await Console.Out.WriteLineAsync("Please enter the message you want to send...");
                                    var message = Console.ReadLine();

                                    await _botBl.SendDMAsync(user, message);
                                    return true;
                                }
                        }
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync("Invalid number of arguments!");
                        return true;
                    }

                    return true;
                case CommandsEnum.richpresence:
                    commandComponents.Remove("richpresence");

                    if (commandComponents.Count() == 2 || commandComponents.Count() == 3)
                    {
                        await _botBl.UpdateRichPresence(
                            commandComponents[1],
                            (ActivityType)Enum.Parse(typeof(ActivityType), commandComponents[0]),
                            commandComponents.Count() == 2 ? null : commandComponents[2]);

                        await Console.Out.WriteLineAsync("Updated successfully");
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync("Invalid number of arguments!");
                        return true;
                    }
                    return true;
                
            }

            return true;
        }
    }
}
