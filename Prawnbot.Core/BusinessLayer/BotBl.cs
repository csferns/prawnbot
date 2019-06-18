using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Log;
using Prawnbot.Core.Module;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Core.Utility;
using Prawnbot.Data.Entities;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IBotBl
    {
        /// <summary>
        /// Connects the bot using a given token
        /// </summary>
        /// <param name="token">The token to connect with</param>
        /// <returns></returns>
        Task<bool> ConnectAsync(string token);
        /// <summary>
        /// Method to disconnect the bot
        /// </summary>
        /// <returns></returns>
        Task<bool> DisconnectAsync(bool switchBot = false);
        /// <summary>
        /// Disconnects and Reconnects the bot
        /// </summary>
        /// <returns></returns>
        Task<bool> ReconnectAsync();
        /// <summary>
        /// Event handler to handle the messages that come in
        /// </summary>
        /// <param name="arg">Message to pass into the commands</param>
        /// <returns></returns>
        Task<bool> HandleCommandAsync(SocketMessage arg);
        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        Task<bool> SetBotStatusAsync(UserStatus status);
        /// <summary>
        /// Updates the bot's rich presence
        /// </summary>
        /// <param name="name">Name of the game</param>
        /// <param name="activityType">Activity type of the game</param>
        /// <param name="streamUrl">(Optional) stream url</param>
        /// <returns></returns>
        Task UpdateRichPresence(string name, ActivityType activityType, string streamUrl);
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        SocketGuildUser GetUser(string username);
        /// <summary>
        /// Get all the current users in the servers the bot is connected to
        /// </summary>
        /// <returns></returns>
        List<SocketGuildUser> GetAllUsers();
        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        Task<List<IMessage>> GetAllMessages(ulong id);
        /// <summary>
        /// Get a guild by name
        /// </summary>
        /// <param name="guildName">Name of the guild</param>
        /// <returns></returns>
        SocketGuild GetGuild(string guildName);
        /// <summary>
        /// Get all the current guilds the bot is connected to
        /// </summary>
        /// <returns>List of IGuilds</returns>
        List<SocketGuild> GetAllGuilds();
        /// <summary>
        /// Gets the default channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel FindDefaultChannel(SocketGuild guild);
        /// <summary>
        /// Gets a text channel with the supplied ID
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(ulong id);
        /// <summary>
        /// Gets a text channel with the supplied guild and id
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, ulong id);
        /// <summary>
        /// Gets a text channel with a supplied guild and name
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, string channelName);
        /// <summary>
        /// Gets all the channels of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>IReadOnlyCollection of SocketTextChannels</returns>
        IReadOnlyCollection<SocketTextChannel> GetGuildTextChannels(SocketGuild guild);
        /// <summary>
        /// Gets a channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <param name="channel">Channel</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel);
        /// <summary>
        /// Create an FFMPEG process for the audio commands
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Process CreateFfmpegProcess(string path);
        /// <summary>
        /// Leaves the current audio channel
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        Task LeaveAudio(IGuild guild);
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="guild">Guild to send it to</param>
        /// <param name="channel">Channel to send it to</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText);
        /// <summary>
        /// Send a message through a DM
        /// </summary>
        /// <param name="user">User to DM</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendDMAsync(SocketGuildUser user, string messageText);
        /// <summary>
        /// Set up the Quartz scheduler for the scheduled jobs
        /// </summary>
        /// <returns></returns>
        Task QuartzSetup();
        /// <summary>
        /// Event that is fired when the client disconnects
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Task.CompletedTask</returns>
        Task Client_Disconnected(Exception arg);
        /// <summary>
        /// Announce to the server a user joined
        /// </summary>
        /// <param name="user">User that joined</param>
        /// <returns></returns>
        Task AnnounceUserJoined(SocketGuildUser user);
        /// <summary>
        /// Announce to the server the user was banned
        /// </summary>
        /// <param name="user">Given user</param>
        /// <param name="guild">Server</param>
        /// <returns>Task</returns>
        Task AnnounceUserBan(SocketUser user, SocketGuild guild);
        /// <summary>
        /// Announce to the server the user was banned
        /// </summary>
        /// <param name="user">Given user</param>
        /// <param name="guild">Server</param>
        /// <returns>Task</returns>
        Task Client_UserUnbanned(SocketUser user, SocketGuild guild);
    }

    public class BotBl : BaseBl, IBotBl
    {
        #region Constructors
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
        #endregion

        public void LayerSetup()
        {
            _apiBl = new APIBl();
            _apiService = new APIService();

            _botBl = new BotBl();
            _botService = new BotService();

            _commandBl = new CommandBl();
            _commandService = new ServiceLayer.CommandService();

            _fileBl = new FileBl();
            _fileService = new FileService();

            _databaseAccessBl = new DatabaseAccessBl();
            _dbAccessService = new DatabaseAccessService();

            _speechRecognitionBl = new SpeechRecognitionBl();
            _speechRecognitionService = new SpeechRecognitionService();

            logging = new Logging();
        }

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

            try
            {
                await _commands.AddModulesAsync(Assembly.Load(typeof(Modules).Assembly.FullName), _services);

                await _client.LoginAsync(TokenType.Bot, _token);
                await _client.StartAsync();
                await SetBotStatusAsync(UserStatus.Online);

                if (!IsQuartzInitialized)
                {
                    await QuartzSetup();
                }
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "ConnectAsync()", "Error connecting the bot", e));
            }

            return true;
        }

        public async Task<bool> DisconnectAsync(bool switchBot = false)
        {
            try
            {
                if (_client != null)
                {
                    await _client.LogoutAsync();
                    await _client.StopAsync();

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

        public async Task<bool> HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                SocketUserMessage message = arg as SocketUserMessage;

                if (message is null || message.Author.IsBot) return false;

                SocketCommandContext context = new SocketCommandContext(_client, message);

                //if (message.ContainsEmoji() && message.Channel.GetType() != typeof(SocketDMChannel)) await context.Channel.SendMessageAsync(message.FindEmoji());

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

        public async Task UpdateRichPresence(string name, ActivityType activityType, string streamUrl)
        {
            try
            {
                await _client.SetGameAsync(name ?? null, activityType == ActivityType.Streaming ? streamUrl : null, activityType);
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "RichPresence", "Error occured while updating rich presence", e));
            }
        }

        public SocketGuildUser GetUser(string username)
        {
            try
            {
                List<SocketGuildUser> users = GetAllUsers();
                return users.Where(x => x.Username == username).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SocketGuildUser> GetAllUsers()
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (SocketGuild guild in _client.Guilds)
            {
                foreach (SocketGuildUser user in guild.Users)
                {
                    users.Add(user);
                }
            }

            return users;
        }

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

        public SocketGuild GetGuild(string guildName)
        {
            try
            {
                return _client.Guilds.Where(x => x.Name == guildName).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<SocketGuild> GetAllGuilds()
        {
            return _client.Guilds.ToList();
        }

        public SocketTextChannel FindDefaultChannel(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).DefaultChannel;
        }

        public SocketTextChannel FindTextChannel(ulong id)
        {
            foreach (SocketGuild guild in _client.Guilds)
            {
                foreach (SocketTextChannel channel in guild.TextChannels)
                {
                    if (channel.Id == id)
                    {
                        return channel;
                    }
                }
            }

            return null;
        }

        public SocketTextChannel FindTextChannel(SocketGuild guild, ulong id)
        {
            return guild.GetTextChannel(id);
        }

        public SocketTextChannel FindTextChannel(SocketGuild guild, string channelName)
        {
            return guild.TextChannels.Where(x => x.Name == channelName).FirstOrDefault();
        }

        public IReadOnlyCollection<SocketTextChannel> GetGuildTextChannels(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels;
        }

        public SocketTextChannel GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels.FirstOrDefault(x => x == channel);
        }

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

        public async Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText)
        {
            await GetGuildTextChannel(guild, channel).SendMessageAsync(messageText);
        }

        public async Task SendDMAsync(SocketGuildUser user, string messageText)
        {
            await UserExtensions.SendMessageAsync(user, messageText);
        }

        public async Task QuartzSetup()
        {
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<MOC>()
                .WithIdentity("MOC")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("MOCTrigger")
                .StartAt(DateBuilder.DateOf(9, 11, 0))
                .WithSimpleSchedule(x => x
                                        .WithIntervalInHours(12)
                                        .RepeatForever())
                .ForJob("MOC")
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            IsQuartzInitialized = true;
        }

        #region Event Listeners
        public async Task Client_Disconnected(Exception arg)
        {
            await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "", "Client disconnected", arg));
        }

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

        public async Task AnnounceUserBan(SocketUser user, SocketGuild guild)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Banned.")
                .WithColor(Color.Red)
                .WithDescription($"{user.Username} was banned from {guild.Name}.")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
        }

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
