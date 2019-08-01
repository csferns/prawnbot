using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Core.Log;
using Prawnbot.Core.Quartz;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Core.Utility;
using Prawnbot.Utility.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
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
        Task ConnectAsync(string token, IContainer autofacContainer);
        /// <summary>
        /// Method to disconnect the bot
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync(bool switchBot = false);
        /// <summary>
        /// Disconnects and Reconnects the bot
        /// </summary>
        /// <returns></returns>
        Task ReconnectAsync();
        /// <summary>
        /// Event handler to handle the messages that come in
        /// </summary>
        /// <param name="arg">Message to pass into the commands</param>
        /// <returns></returns>
        Task HandleCommandAsync(SocketMessage arg);
        /// <summary>
        /// Set up the Quartz scheduler for the scheduled jobs
        /// </summary>
        /// <returns></returns>
        Task QuartzSetupAsync();
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

        private static string Token { get; set; }
        private static bool IsQuartzInitialized { get; set; }

        private static Discord.Commands.CommandService Commands;
        private static IServiceProvider BotServices;

        public async Task ConnectAsync(string token, IContainer autofacContainer = null)
        {
            try
            {
                Token = token;

                Client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose
                });

                Commands = new Discord.Commands.CommandService();
                Commands.AddTypeReader(typeof(IVoiceRegion), new VoiceRegionTypeReader());

                BotServices = new ServiceCollection()
                    .AddSingleton(Client)
                    .AddSingleton(Commands)
                    .AddSingleton(autofacContainer.Resolve<IBotService>())
                    .AddSingleton(autofacContainer.Resolve<ICoreService>())
                    .AddSingleton(autofacContainer.Resolve<IAPIService>())
                    .AddSingleton(autofacContainer.Resolve<IFileService>())
                    .AddSingleton(autofacContainer.Resolve<ISpeechRecognitionService>())
                    .BuildServiceProvider();

                Client.MessageReceived += HandleCommandAsync;
                Client.UserJoined += AnnounceUserJoined;
                Client.UserBanned += AnnounceUserBan;
                Client.UserUnbanned += Client_UserUnbanned;
                Client.Log += logging.PopulateEventLogAsync;
                Client.Disconnected += Client_Disconnected;

                //_unitOfWork = new UnitOfWork(new BotDatabaseContext());

                await Commands.AddModulesAsync(typeof(Modules.Modules).Assembly, BotServices);

                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();
                await coreBL.SetBotStatusAsync(UserStatus.Online);

                if (!IsQuartzInitialized)
                {
                    await QuartzSetupAsync();
                }
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "ConnectAsync()", "Error connecting the bot", e));
            }
        }

        public async Task DisconnectAsync(bool switchBot = false)
        {
            try
            {
                if (Client != null)
                {
                    await Client.LogoutAsync();
                    await Client.StopAsync();

                    Token = switchBot
                        ? null
                        : Token;
                    //workerCancellationTokenSource.Cancel();
                }
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "Disconnect", "Error disconnecting the bot", e));
                return;
            }
        }

        public async Task ReconnectAsync()
        {
            try
            {
                await DisconnectAsync();

                // TODO: handle autofac container on reconnect
                await ConnectAsync(Token);
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "Reconnect", "Error reconnecting the bot", e));
                return;
            }
        }

        public async Task QuartzSetupAsync()
        {
            try
            {
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                IJobDetail mocJob = JobBuilder.Create<MOC>()
                    .WithIdentity("MOC")
                    .Build();

                ITrigger mocTrigger = TriggerBuilder.Create()
                    .WithIdentity("MOCTrigger")
                    .StartAt(DateBuilder.DateOf(9, 11, 0))
                    .WithSimpleSchedule(x => x
                                            .WithIntervalInHours(12)
                                            .RepeatForever())
                    .ForJob("MOC")
                    .Build();

                await scheduler.ScheduleJob(mocJob, mocTrigger);
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "QuartzSetup", $"Job {typeof(MOC).FullName} scheduled"));

                IJobDetail yearlyQuoteJob = JobBuilder.Create<YearlyQuote>()
                    .WithIdentity("YearlyQuote")
                    .Build();

                ITrigger yearlyQuoteTrigger = TriggerBuilder.Create()
                    .WithIdentity("YearlyQuoteTrigger")
                    .StartAt(DateBuilder.DateOf(8, 0, 0))
                    .WithSimpleSchedule(x => x
                                            .WithIntervalInHours(24)
                                            .RepeatForever())
                    .ForJob("YearlyQuote")
                    .Build();

                await scheduler.ScheduleJob(yearlyQuoteJob, yearlyQuoteTrigger);
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "QuartzSetup", $"Job {typeof(YearlyQuote).FullName} scheduled"));

                IsQuartzInitialized = true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "QuartzSetup", $"An error occured while scheduling Quartz jobs", e));
            }
        }

        #region Event Listeners
        public async Task HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                SocketUserMessage message = arg as SocketUserMessage;

                if (message is null || message.Author.IsBot) return;

                SocketCommandContext context = new SocketCommandContext(Client, message);

                int argPos = 0;

                if (message.HasStringPrefix(ConfigUtility.CommandDelimiter, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || context.IsPrivate)
                {
                    IResult result = await Commands.ExecuteAsync(context, argPos, BotServices);

                    if (message.Channel.GetType() == typeof(SocketDMChannel)) await logging.PopulateMessageLogAsync(new LogMessage(LogSeverity.Info, "Message", $"{message.Author.Username}: {message.Content}"));

                    if (!result.IsSuccess && result.ErrorReason != null)
                    {
                        await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }

                    await logging.LogCommandUseAsync(context.Message.Author.Username, context.Guild.Name, context.Message.Content);
                }
                else
                {
                    if (ConfigUtility.AllowEventListeners)
                    {
                        Context = context;

                        if (message.ContainsEmote() && ConfigUtility.EmojiRepeat) await context.Channel.SendMessageAsync(coreBL.FindEmojis(message));
                        await coreBL.ContainsTextAsync(message);
                        await coreBL.ContainsUserAsync(message);
                    }
                }
            }
            catch (Exception e)
            {
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "HandleCommand", "Error handling the command", e));
                return;
            }

        }
        public async Task Client_Disconnected(Exception arg)
        {
            await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Error, "", "Client disconnected", arg));
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
