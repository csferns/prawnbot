using Autofac;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Interfaces;
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
    public class BotBL : BaseBL, IBotBL
    {
        private readonly ICoreBL coreBL;
        private readonly ILogging logging;
        private readonly IConfigUtility configUtility;

        public BotBL(ICoreBL coreBL, ILogging logging, IConfigUtility configUtility)
        {
            this.coreBL = coreBL;
            this.logging = logging;
            this.configUtility = configUtility;
        }

        private static string Token { get; set; }
        private static IContainer AutofacContainer { get; set; }
        private CommandService Commands { get; set; }
        private IServiceProvider BotServices { get; set; }

        private IScheduler Scheduler;

        private bool IsQuartzInitialized 
        { 
            get
            {
                return Scheduler != null && !(Scheduler?.IsShutdown ?? true) && (Scheduler?.IsStarted ?? false) && !(Scheduler?.InStandbyMode ?? true);
            }
        }

        public object GetStatus() 
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

        public async Task ConnectAsync(string token = null, IContainer autofacContainer = null)
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
                    .AddSingleton(autofacContainer.Resolve<ILogging>())
                    .AddSingleton(autofacContainer.Resolve<IAlarmService>())
                    .AddSingleton(autofacContainer.Resolve<ISpeechRecognitionService>())
                    .BuildServiceProvider();

                Client.MessageReceived += Client_MessageRecieved;
                Client.Log += logging.Client_Log; 
                Client.Disconnected += Client_Disconnected;
                Client.Connected += Client_Connected;

                if (configUtility.AllowEventListeners)
                {
                    if (configUtility.UserJoined) { Client.UserJoined += Client_UserJoined; }
                    if (configUtility.UserBanned) { Client.UserBanned += Client_UserBanned; }
                    if (configUtility.UserUnbanned) { Client.UserUnbanned += Client_UserUnbanned; }
                    if (configUtility.JoinedGuild) { Client.JoinedGuild += Client_JoinedGuild; }
                    if (configUtility.MessageDeleted) { Client.MessageDeleted += Client_MessageDeleted; }
                }

                await Commands.AddModulesAsync(typeof(Modules.Modules).Assembly, BotServices);

                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();
                await coreBL.SetBotStatusAsync();

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

        public async Task DisconnectAsync(bool shutdown = false)
        {
            try
            {
                if (Client != null)
                {
                    await Client.LogoutAsync();

                    if (shutdown)
                    {
                        await Client.StopAsync();

                        Client.Dispose();
                        AutofacContainer.Dispose();

                        ShutdownQuartz();
                    }
                }
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
            }
        }

        /// <summary>
        /// Shuts down the Quartz setup
        /// </summary>
        public void ShutdownQuartz()
        {
            if (IsQuartzInitialized)
            {
                Scheduler.Shutdown();
                Scheduler = null;
            }
        }

        public async Task ReconnectAsync()
        {
            try
            {
                await DisconnectAsync();
                await ConnectAsync();
            }
            catch (Exception e)
            {
                await logging.Log_Exception(e);
                return;
            }
        }

        /// <summary>
        /// Sets up all the Quartz jobs
        /// </summary>
        /// <returns>awaitable Task</returns>
        public async Task QuartzSetupAsync()
        {
            try
            {
                // Initialise the things Quartz needs to work properly
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props); 
                Scheduler = await factory.GetScheduler();

                // Start the scheduler 
                await Scheduler.Start();

                // Custom job factory to allow for injecting dependencies into Quartz jobs
                Scheduler.JobFactory = new QuartzJobFactory(AutofacContainer);

                // TODO: Add a job in here to change the channel names to include seasonal emojis
                // Create / Schedule Quartz jobs
                int TWELVE_HOUR = 12;
                int TWENTYFOUR_HOUR = 24;

                await ScheduleQuartzJob<MOCJob>(DateBuilder.DateOf(9, 11, 0), interval: TWELVE_HOUR);
                await ScheduleQuartzJob<YearlyQuoteJob>(DateBuilder.DateOf(8, 0, 0), interval: TWENTYFOUR_HOUR);
                await ScheduleQuartzJob<BirthdayJob>(DateBuilder.DateOf(8, 0, 0), interval: TWENTYFOUR_HOUR);
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

        /// <summary>
        /// Schedule a Quartz job to be run
        /// </summary>
        /// <typeparam name="T">Class that implements IJob</typeparam>
        /// <param name="startDate">Start date of the Job</param>
        /// <param name="interval">Interval in which the job runs</param>
        /// <returns>awaitable Task</returns>
        public async Task ScheduleQuartzJob<T>(DateTimeOffset startDate, int interval) where T : IJob
        {
            string jobName = typeof(T).Name;  

            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName + "Trigger")
                .StartAt(startDate)
                .WithSimpleSchedule(x => x
                                        .WithIntervalInHours(interval)
                                        .RepeatForever())
                .ForJob(jobName)
                .Build();

            await Scheduler.ScheduleJob(job, trigger);

            bool jobExists = await Scheduler.CheckExists(trigger.Key);

            if (jobExists)
            {
                await logging.Log_Info($"Job {jobName} scheduled for {trigger.StartTimeUtc.ToString("dd/MM/yyyy HH:mm:ss")} with interval {interval} hours");
            }
        }

        public async Task SetBotRegionAsync(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                await Context.Channel.SendMessageAsync("Region cannot be empty");
                return;
            }

            IEnumerable<RestVoiceRegion> regions = await Context.Guild.GetVoiceRegionsAsync().ToAsyncEnumerable().FlattenAsync();
            bool validRegion = regions.Any(x => x.Name == regionName);

            if (!validRegion)
            {
                await Context.Channel.SendMessageAsync($"\"{regionName}\" is not a valid region, or the server cannot access this region.");
                return;
            }

            RestVoiceRegion region = regions.FirstOrDefault(x => x.Name == regionName);

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
                .WithDescription($"Hello, my name is {arg.CurrentUser.Nickname} To find out commands, please use {configUtility.CommandDelimiter}commands")
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

                if (message.HasStringPrefix(configUtility.CommandDelimiter, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || Context.IsPrivate)
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
                    if (configUtility.AllowEventListeners)
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
            await Client.StopAsync();

            Client.Dispose();

            ShutdownQuartz();

            await ConnectAsync();
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;

            await logging.Log_Info($"User {user.Username} joined {guild.Name}");

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Welcome!")
                .WithColor(Color.Green)
                .WithDescription($"Welcome to {guild.Name}, {user.Username}! The server now has {guild.MemberCount} members")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
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

            await guild.DefaultChannel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }
        #endregion Event Listeners
    }
}
