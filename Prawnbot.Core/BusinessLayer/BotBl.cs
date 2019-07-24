using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Log;
using Prawnbot.Core.Utility;
using Prawnbot.Utility.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface IBotBL
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

    public class BotBL : BaseBL, IBotBL 
    {
        private readonly ICoreBL coreBL;
        private readonly ILogging logging;
        public BotBL(ICoreBL coreBL, ILogging logging)
        {
            this.coreBL = coreBL;
            this.logging = logging;
        }

        private static string _token;
        private static bool IsQuartzInitialized { get; set; }

        private static Discord.Commands.CommandService Commands;
        private static IServiceProvider BotServices;

        public async Task<bool> ConnectAsync(string token)
        {
            try
            {
                _token = token;

                Client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose
                });

                Commands = new Discord.Commands.CommandService();
                Commands.AddTypeReader(typeof(IVoiceRegion), new VoiceRegionTypeReader());

                BotServices = new ServiceCollection()
                    .AddSingleton(Client)
                    .AddSingleton(Commands)
                    .BuildServiceProvider();

                Client.MessageReceived += HandleCommandAsync;
                Client.UserJoined += AnnounceUserJoined;
                Client.UserBanned += AnnounceUserBan;
                Client.UserUnbanned += Client_UserUnbanned;
                Client.Log += logging.PopulateEventLog;
                Client.Disconnected += Client_Disconnected;

                //_unitOfWork = new UnitOfWork(new BotDatabaseContext());

                //await Commands.AddModulesAsync(Assembly.Load(typeof(Modules.Modules).Assembly.FullName), BotServices);

                await Client.LoginAsync(TokenType.Bot, _token);
                await Client.StartAsync();
                await coreBL.SetBotStatusAsync(UserStatus.Online);

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
                if (Client != null)
                {
                    await Client.LogoutAsync();
                    await Client.StopAsync();

                    _token = switchBot 
                        ? string.Empty
                        : null;
                    //else workerCancellationTokenSource.Cancel();
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

                SocketCommandContext context = new SocketCommandContext(Client, message);

                int argPos = 0;

                if (message.HasStringPrefix("p!", ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || context.IsPrivate)
                {
                    IResult result = await Commands.ExecuteAsync(context, argPos, BotServices);

                    if (message.Channel.GetType() == typeof(SocketDMChannel)) await logging.PopulateMessageLog(new LogMessage(LogSeverity.Info, "Message", $"{message.Author.Username}: {message.Content}"));

                    if (!result.IsSuccess) await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));

                    if (
                        result.ErrorReason != null
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
                        Context = context;

                        if ((message.ContainsEmote() || message.ContainsEmoji()) && ConfigUtility.EmojiRepeat) await context.Channel.SendMessageAsync(coreBL.FindEmojis(message));
                        await coreBL.ContainsText(message);
                        await coreBL.ContainsUser(message);
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

        public async Task QuartzSetup()
        {
            await Prawnbot.Core.Quartz.QuartzSetup.Setup();

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
