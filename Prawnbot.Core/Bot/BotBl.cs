﻿using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Base;
using Prawnbot.Core.Log;
using Prawnbot.Core.Module;
using Prawnbot.Core.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.Bot
{
    public interface IBotBl
    {
        Task<bool> ConnectAsync(string token);
        Task<bool> DisconnectAsync(bool switchBot = false);
        Task<bool> ReconnectAsync();
        Task<bool> SetBotStatusAsync(UserStatus status);
        List<SocketGuildUser> GetAllUsers();
        Task<List<IMessage>> GetAllMessages(ulong id);
        List<SocketGuild> GetAllGuilds();
        SocketTextChannel GetDefaultChannel(SocketGuild guild);
        SocketTextChannel GetChannelById(ulong id);
        IReadOnlyCollection<SocketTextChannel> GetGuildTextChannels(SocketGuild guild);
        SocketTextChannel GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel);
        Process CreateFfmpegProcess(string path);
        Task Client_Connected();
        Task Client_Disconnected(Exception arg);
        Task AnnounceUserJoined(SocketGuildUser user);
        Task AnnounceUserBan(SocketUser user, SocketGuild guild);
        Task Client_UserUnbanned(SocketUser user, SocketGuild guild);
        Task LeaveAudio(IGuild guild);
    }

    public class BotBl : BaseBl, IBotBl
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        private static DiscordSocketClient _client;
        /// <summary>
        /// The static connection state of the client
        /// </summary>
        private static ConnectionState connectionState;
        /// <summary>
        /// The static token the bot uses to connect
        /// </summary>
        private static string _token;
        /// <summary>
        /// The collection of the commands the bot can be sent
        /// </summary>
        private Discord.Commands.CommandService _commands;
        /// <summary>
        /// Collection of services the bot can use
        /// </summary>
        private IServiceProvider _services;
        /// <summary>
        /// The context of the message
        /// </summary>
        private SocketCommandContext Context;

        private CancellationTokenSource cancellationTokenSource;

        public BotBl()
        {
             
        }

        /// <summary>
        /// Creates an instance of the BotBl with the Command Context
        /// </summary>
        /// <param name="_context"></param>
        public BotBl(SocketCommandContext _context)
        {
            Context = _context;
        }

        /// <summary>
        /// Sets up each layer of the application
        /// </summary>
        public void LayerSetup()
        {
            logging = new Logging();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false);

            ConfigUtility = new ConfigUtility(builder.Build());

            _apiBl = new API.APIBl();
            _apiService = new API.APIService();

            _botBl = new Bot.BotBl();
            _botService = new Bot.BotService();

            _commandBl = new Command.CommandBl();
            _commandService = new Command.CommandService();

            _fileBl = new LocalFileAccess.FileBl();
            _fileService = new LocalFileAccess.FileService();

            _dbAccessBl = new DatabaseAccess.DatabaseAccessBl();
            _dbAccessService = new DatabaseAccess.DatabaseAccessService();

            _speechRecognitionBl = new SpeechRecognition.SpeechRecognitionBl();
            _speechRecognitionService = new SpeechRecognition.SpeechRecognitionService();
        }

        /// <summary>
        /// Connects the bot using a given token
        /// </summary>
        /// <param name="token">The token to connect with</param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string token)
        {
            LayerSetup();

            _token = token;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _commands = new Discord.Commands.CommandService();
            _commands.AddTypeReader(typeof(IVoiceRegion), new VoiceRegionTypeReader());

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += AnnounceUserJoined;
            _client.UserBanned += AnnounceUserBan;
            _client.UserUnbanned += Client_UserUnbanned;
            _client.Log += logging.PopulateEventLog;
            _client.Disconnected += Client_Disconnected;
            _client.Connected += Client_Connected;

            try
            {
                await _commands.AddModulesAsync(Assembly.Load(typeof(Modules).Assembly.FullName), _services);

                await _client.LoginAsync(TokenType.Bot, _token);
                await _client.StartAsync();
                await SetBotStatusAsync(UserStatus.Online); 
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "ConnectAsync()", "Error connecting the bot", e));
            }

            return true;
        }

        /// <summary>
        /// Method to disconnect the bot
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DisconnectAsync(bool switchBot = false)
        {
            try
            {
                if (_client != null)
                {
                    await _client.LogoutAsync();
                    await _client.StopAsync();

                    connectionState = ConnectionState.Disconnected;
                    if (switchBot) _token = null;
                }

                return true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "Disconnect", "Error disconnecting the bot", e));
                return false;
            }

        }

        public async Task<bool> ReconnectAsync()
        {
            try
            {
                await DisconnectAsync();
                await ConnectAsync(_token);

                return true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "Reconnect", "Error reconnecting the bot", e));
                return false;
            }
        }

        /// <summary>
        /// Event handler to handle the messages that come in
        /// </summary>
        /// <param name="arg">Message to pass into the commands</param>
        /// <returns></returns>
        public async Task<bool> HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                SocketUserMessage message = arg as SocketUserMessage;

                if (message is null || message.Author.IsBot) return false;

                SocketCommandContext context = new SocketCommandContext(_client, message);

                int argPos = 0;

                if (message.HasStringPrefix("p!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) || context.IsPrivate)
                {
                    IResult result = await _commands.ExecuteAsync(context, argPos, _services);

                    if (message.Channel.GetType() == typeof(SocketDMChannel)) await logging.PopulateMessageLog(new LogMessage(LogSeverity.Info, "Message", $"{message.Author.Username}: {message.Content}"));

                    if (!result.IsSuccess) await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));

                    if (
                        result.ErrorReason != null
                        && result.ErrorReason.ToLowerInvariant().Contains("unknown")
                        && message.Channel.GetType() != typeof(SocketDMChannel)
                       )
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
                else
                {
                    if (ConfigUtility.AllowEventListeners)
                    {
                        await _commandService.ContainsText(context, message);
                        await _commandService.ContainsUser(context, message);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "HandleCommand", "Error handling the command", e));
                return false;
            }

        }

        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        public async Task<bool> SetBotStatusAsync(UserStatus status)
        {
            try
            {
                await _client.SetStatusAsync(status);
                return true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "BotStatus", "Error setting bot status", e));
                return false;
            }

        }

        /// <summary>
        /// Get all the current users in the servers the bot is connected to
        /// </summary>
        /// <returns></returns>
        public List<SocketGuildUser> GetAllUsers()
        {
            return Context.Guild.Users.ToList();
        }

        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        public async Task<List<IMessage>> GetAllMessages(ulong id)
        {
            RequestOptions options = new RequestOptions
            {
                Timeout = 2000,
                RetryMode = RetryMode.RetryTimeouts,
                CancelToken = CancellationToken.None
            };
            IEnumerable<IMessage> messages = await Context.Guild.GetTextChannel(id).GetMessagesAsync(limit: 500000, options: options).FlattenAsync();

            return messages.Reverse().ToList();
        }

        /// <summary>
        /// Get all the current guilds the bot is connected to
        /// </summary>
        /// <returns>List of IGuilds</returns>
        public List<SocketGuild> GetAllGuilds()
        {
            return _client.Guilds.ToList();
        }

        /// <summary>
        /// Gets the default channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>SocketTextChannel</returns>
        public SocketTextChannel GetDefaultChannel(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).DefaultChannel;
        }

        /// <summary>
        /// Gets a text channel with the supplied ID
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <returns></returns>
        public SocketTextChannel GetChannelById(ulong id)
        {
            return Context.Guild.GetTextChannel(id);
        }

        /// <summary>
        /// Gets all the channels of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>IReadOnlyCollection of SocketTextChannels</returns>
        public IReadOnlyCollection<SocketTextChannel> GetGuildTextChannels(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels;
        }

        /// <summary>
        /// Gets a channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <param name="channel">Channel</param>
        /// <returns>SocketTextChannel</returns>
        public SocketTextChannel GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels.FirstOrDefault(x => x == channel);
        }

        /// <summary>
        /// Create an FFMPEG process for the audio commands
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Process CreateFfmpegProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        #region Event Listeners
        /// <summary>
        /// Event that is fired when the client connects
        /// </summary>
        /// <returns>Task</returns>
        public async Task Client_Connected()
        {
            connectionState = ConnectionState.Connected;

            cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                while (connectionState == ConnectionState.Connected)
                {
                    await Task.Delay(60000);

                    if (
                        (DateTime.Now.Hour == 9 || DateTime.Now.Hour == 21)
                        && DateTime.Now.Minute == 11
                        )
                    {
                        foreach (var guild in _client.Guilds)
                        {
                            await guild.DefaultChannel.SendMessageAsync("Happy meme o'clock!");
                        }

                        break;
                    }
                }

                await ReconnectAsync();

                cancellationTokenSource.Cancel();

            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Event that is fired when the client disconnects
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Task.CompletedTask</returns>
        public async Task Client_Disconnected(Exception arg)
        {
            await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "", "Client disconnected", arg));
            connectionState = ConnectionState.Disconnected;
        }

        /// <summary>
        /// Announce to the server a user joined
        /// </summary>
        /// <param name="user">User that joined</param>
        /// <returns></returns>
        public async Task AnnounceUserJoined(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            SocketTextChannel channel = guild.DefaultChannel;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Welcome!")
                .WithColor(Color.Green)
                .WithDescription($"Welcome to {guild.Name}, {user.Username}! The server now has {guild.MemberCount} members");

            await channel.SendMessageAsync("", false, builder.Build());
        }

        /// <summary>
        /// Announce to the server the user was banned
        /// </summary>
        /// <param name="user">Given user</param>
        /// <param name="guild">Server</param>
        /// <returns>Task</returns>
        public async Task AnnounceUserBan(SocketUser user, SocketGuild guild)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Banned.")
                .WithColor(Color.Red)
                .WithDescription($"{user.Username} was banned from {guild.Name}.")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
        }

        /// <summary>
        /// Announce to the server the user was banned
        /// </summary>
        /// <param name="user">Given user</param>
        /// <param name="guild">Server</param>
        /// <returns>Task</returns>
        public async Task Client_UserUnbanned(SocketUser user, SocketGuild guild)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Unbanned.")
                .WithColor(Color.Green)
                .WithDescription($"{user.Username} was unbanned from {guild.Name}.")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
        }
        #endregion Event Listeners
    }
}