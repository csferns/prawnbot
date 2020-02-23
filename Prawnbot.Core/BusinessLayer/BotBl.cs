using Autofac;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Log;
using Prawnbot.Core.Quartz;
using Prawnbot.Core.ServiceLayer;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
        Task ConnectAsync(string token, IContainer container = null);
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
        /// Set up the Quartz scheduler for the scheduled jobs
        /// </summary>
        /// <returns></returns>
        Task QuartzSetupAsync();
        /// <summary>
        /// Event that is fired when the client disconnects
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Task.CompletedTask</returns>
        Task<object> GetStatusAsync();
        Task SetBotRegionAsync(string regionName);
        void ShutdownQuartz();
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

        private string Token { get; set; }
        private IContainer AutofacContainer { get; set; }
        private CommandService Commands { get; set; }
        private IServiceProvider BotServices { get; set; }

        private IScheduler Scheduler;

        private bool IsQuartzInitialized 
        { 
            get
            {
                return Scheduler != null && !Scheduler.IsShutdown && Scheduler.IsStarted && !Scheduler.InStandbyMode;
            }
        }

        public async Task<object> GetStatusAsync() 
        {
            string offlineString = "Offline";

            return new
            {
                Online = Client != null && Client?.ConnectionState == ConnectionState.Connected,
                ConnectionState = Client?.ConnectionState ?? ConnectionState.Disconnected,
                Status = Client?.Status ?? UserStatus.Offline,
                Activity = Client?.Activity.Name ?? offlineString,
                User = Client?.CurrentUser.Username ?? offlineString,
                Token = Token ?? offlineString,
                Ping = Client?.Latency ?? 0,
                ConnectedServers = Client?.Guilds.ToArray() ?? Array.Empty<SocketGuild>()
            };
        }

        public async Task ConnectAsync(string token = "", IContainer autofacContainer = null)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Process currentProcess = Process.GetCurrentProcess();

                await logging.Log_Info($"Process {currentProcess.ProcessName} ({currentProcess.Id}) started on {Environment.MachineName} at {currentProcess.StartTime}");

                Token ??= token;
                AutofacContainer ??= autofacContainer;

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

                Client.MessageReceived += Client_MessageRecieved;
                Client.Log += logging.Client_Log; 
                Client.Disconnected += Client_Disconnected;
                Client.Connected += Client_Connected;

                if (ConfigUtility.AllowNonEssentialListeners)
                {                
                    Client.UserJoined += Client_UserJoined;
                    Client.UserBanned += Client_UserBanned;
                    Client.UserUnbanned += Client_UserUnbanned;
                    Client.JoinedGuild += Client_JoinedGuild;
                    Client.MessageDeleted += Client_MessageDeleted;
                }

                await Commands.AddModulesAsync(typeof(Modules.Modules).Assembly, BotServices);

                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();
                await coreBL.SetBotStatusAsync(UserStatus.Online);

                if (!IsQuartzInitialized)
                {
                    await QuartzSetupAsync();
                }

                stopwatch.Stop();

                await logging.Log_Debug($"Bot started in {stopwatch.Elapsed}");

                await logging.Log_Debug($"Memory used before collection: {(GC.GetTotalMemory(false) / 1024) / 1024} MB");
                GC.Collect();
                await logging.Log_Debug($"Memory used after collection: {(GC.GetTotalMemory(true) / 1024) / 1024} MB");
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
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
                await logging.Log_Exception(e);
            }
        }

        public async Task ReconnectAsync()
        {
            try
            {
                await DisconnectAsync();
                await ConnectAsync(Token, AutofacContainer);
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
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
                Scheduler = await factory.GetScheduler();

                // and start it off
                await Scheduler.Start();

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

                await Scheduler.ScheduleJob(mocJob, mocTrigger);
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

                await Scheduler.ScheduleJob(yearlyQuoteJob, yearlyQuoteTrigger);
                await logging.PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "QuartzSetup", $"Job {typeof(YearlyQuote).FullName} scheduled"));
            }
            catch (SchedulerException sc)
            {
                await logging.Log_Exception(sc, optionalMessage: "A SchedulerExeption occured while scheduling Quartz jobs");
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
            }
        }

        public void ShutdownQuartz()
        {
            if (IsQuartzInitialized)
            {
                Scheduler.Shutdown();
            }
        }

        public async Task SetBotRegionAsync(string regionName)
        {
            if (regionName == null)
            {
                await Context.Channel.SendMessageAsync("Region cannot be empty");
                return;
            }

            IEnumerable<RestVoiceRegion> regions = await Context.Guild.GetVoiceRegionsAsync().ToAsyncEnumerable().FlattenAsync();
            RestVoiceRegion region = regions.FirstOrDefault(x => x.Name == regionName);

            bool validRegion = regions.Any(x => x.Id == region.Id);

            if (!validRegion)
            {
                await Context.Channel.SendMessageAsync($"\"{regionName}\" is not a valid region, or the server cannot access this region.");
                return;
            }

            await Context.Channel.SendMessageAsync($"Setting server {Format.Bold(Context.Guild.Name)}'s region to {region}");

            Optional<IVoiceRegion> optionalRegion = new Optional<IVoiceRegion>(region);
            await Context.Guild.ModifyAsync(x => x.Region = optionalRegion);
        }

        #region Event Listeners
        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel channel)
        {
            await logging.Log_Info($"Message deleted {arg1.Id} from {channel.Name}");
            await channel.SendMessageAsync("Oooh, he's deletin'", UseTTS);
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            await logging.Log_Info($"Joined guild {arg.Id} ({arg.Name})");

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithAuthor(Client.CurrentUser)
                .WithColor(Color.Green)
                .WithDescription($"Hello, my name is Big Succ! To find out commands, please use {ConfigUtility.CommandDelimiter}commands")
                .WithCurrentTimestamp();

            await arg.DefaultChannel.SendMessageAsync(string.Empty, UseTTS, embedBuilder.Build());
        }

        private async Task Client_MessageRecieved(SocketMessage arg)
        {
            try
            {
                if (!(arg is SocketUserMessage message) || message.Author.IsBot || string.IsNullOrWhiteSpace(message.Content))
                {
                    return;
                }

                Context = new SocketCommandContext(Client, message);

                int argPos = 0;

                if (message.HasStringPrefix(ConfigUtility.CommandDelimiter, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || Context.IsPrivate)
                {
                    IResult result = await Commands.ExecuteAsync(Context, argPos, BotServices);

                    if (message.Channel.GetType() == typeof(SocketDMChannel))
                    {
                        await logging.Log_Info($"{message.Author.Username}: {message.Content}");
                    }

                    if (!result.IsSuccess && result.ErrorReason != null)
                    {
                        await logging.Log_Warning(result.ErrorReason);
                        await Context.Channel.SendMessageAsync(result.ErrorReason);
                    }

                    await logging.Log_Info($"Message recieved from {Context.Message.Author.Username} ({Context.Guild.Name}): \"{Context.Message.Content}\"");
                }
                else
                {
                    if (ConfigUtility.AllowEventListeners)
                    {
                        await coreBL.MessageEventListeners(message);
                    }
                }
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e, optionalMessage: "Error handling the command");
                return;
            }
        }

        private async Task Client_Connected()
        {
            await logging.Log_Info($"Connected as {Client.CurrentUser.Username}");
        }

        private async Task Client_Disconnected(Exception arg)
        {
            await logging.Log_Exception(arg, optionalMessage: "Client disconnected");
            await Client.LogoutAsync();
            Client = null;
            ShutdownQuartz();

            await ConnectAsync();
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        { 
            SocketGuild guild = user.Guild;
            SocketTextChannel channel = guild.DefaultChannel;
            await logging.Log_Info($"User {user.Username} joined {guild.Name}");

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Welcome!")
                .WithColor(Color.Green)
                .WithDescription($"Welcome to {guild.Name}, {user.Username}! The server now has {guild.MemberCount} members");

            await channel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        private async Task Client_UserBanned(SocketUser user, SocketGuild guild)
        {
            string message = $"{user.Username} was banned from {guild.Name}.";

            await logging.Log_Info(message);

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Banned.")
                .WithColor(Color.Red)
                .WithDescription(message)
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        private async Task Client_UserUnbanned(SocketUser user, SocketGuild guild)
        {
            string message = $"{user.Username} was unbanned from {guild.Name}.";

            await logging.Log_Info(message);

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Unbanned.")
                .WithColor(Color.Green)
                .WithDescription(message)
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", UseTTS, builder.Build());
        }
        #endregion Event Listeners
    }
}
