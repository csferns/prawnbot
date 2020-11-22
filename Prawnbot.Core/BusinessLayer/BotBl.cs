using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Quartz;
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
        private readonly ILogger<BotBL> logger;
        private readonly IConfigUtility configUtility;

        public BotBL(ICoreBL coreBL, ILogger<BotBL> logger, IConfigUtility configUtility)
        {
            this.coreBL = coreBL;
            this.logger = logger;
            this.configUtility = configUtility;
        }

        private static string Token { get; set; }
        private static IServiceProvider ServiceProvider { get; set; }
        private CommandService Commands { get; set; }

        private IScheduler Scheduler;

        private bool IsQuartzInitialized => Scheduler != null && (!Scheduler.IsShutdown || Scheduler.IsStarted || !Scheduler.InStandbyMode);

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

        public async Task ConnectAsync(string token = null, IServiceProvider serviceProvider = null)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                Process currentProcess = Process.GetCurrentProcess();

                logger.LogInformation("Process {0} ({1}) started on {2} at {3}", currentProcess.ProcessName, currentProcess.Id, Environment.MachineName, currentProcess.StartTime);

                Token ??= token;

                Client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose
                });

                Commands = new CommandService();
                Commands.AddTypeReader(typeof(IVoiceRegion), new VoiceRegionTypeReader());

                IServiceCollection botServices = new ServiceCollection()
                    .AddSingleton(Client)
                    .AddSingleton(Commands);

                if (serviceProvider != null)
                {
                    botServices.AddSingleton(serviceProvider.GetService<IBotService>())
                               .AddSingleton(serviceProvider.GetService<ICoreService>())
                               .AddSingleton(serviceProvider.GetService<IAPIService>())
                               .AddSingleton(serviceProvider.GetService<IFileService>())
                               .AddSingleton(serviceProvider.GetService<ILogger<Modules.Modules>>())
                               .AddSingleton(serviceProvider.GetService<IAlarmService>())
                               .AddSingleton(serviceProvider.GetService<ISpeechRecognitionService>());
                }

                Client.MessageReceived += Client_MessageRecieved;
                Client.Log += Client_Log;
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

                ServiceProvider provider = botServices.BuildServiceProvider();

                await Commands.AddModulesAsync(typeof(Modules.Modules).Assembly, provider);

                ServiceProvider = provider;

                await Client.LoginAsync(TokenType.Bot, Token);
                await Client.StartAsync();
                await coreBL.SetBotStatusAsync();

                if (!IsQuartzInitialized)
                {
                    await QuartzSetupAsync();
                }

                stopwatch.Stop();

                logger.LogDebug("Bot started in {0}", stopwatch.Elapsed);
                logger.LogDebug("Memory used before collection: {0} MB", (GC.GetTotalMemory(false) / 1024) / 1024);
                GC.Collect();
                logger.LogDebug("Memory used after collection: {0} MB", (GC.GetTotalMemory(true) / 1024) / 1024);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured in connecting the bot: {0}", e.Message);
            }
        }

        private Task Client_Log(LogMessage arg)
        {
            bool success = Enum.TryParse<LogLevel>(arg.Severity.ToString(), out LogLevel logLevel);

            if (success)
            {
                logger.Log(logLevel, arg.Exception?.Message ?? arg.Message);
            }

            return Task.CompletedTask;
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

                        ShutdownQuartz();
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured in disconnecting the bot: {0}", e.Message);
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
                logger.LogError(e, "An error occured in reconnecting the bot: {0}", e.Message);
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
                Scheduler.JobFactory = new QuartzJobFactory(logger, ServiceProvider);

                // TODO: Add a job in here to change the channel names to include seasonal emojis
                // Create / Schedule Quartz jobs
                int TWELVE_HOUR = 12;
                int TWENTYFOUR_HOUR = 24;

                await ScheduleQuartzJob<MOCJob>(DateBuilder.DateOf(9, 11, 0), interval: TWELVE_HOUR);
                //await ScheduleQuartzJob<YearlyQuoteJob>(DateBuilder.DateOf(8, 0, 0), interval: TWENTYFOUR_HOUR);
                await ScheduleQuartzJob<BirthdayJob>(DateBuilder.DateOf(8, 0, 0), interval: TWENTYFOUR_HOUR);
            }
            catch (SchedulerException sc)
            {
                logger.LogError(sc, "A SchedulerExeption occured while scheduling Quartz jobs: {0}", sc.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured in setting up Quartz: {0}", e.Message);
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
                logger.LogInformation("Job {0} scheduled for {1:dd/MM/yyyy HH:mm:ss} with interval {2} hours", jobName, trigger.StartTimeUtc, interval);
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
            logger.LogInformation("Message deleted {0} from {1}", arg1.Id, channel.Name);
            await channel.SendMessageAsync("Oooh, he's deletin'", UseTTS);
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            logger.LogInformation("Joined guild {0} ({1})", arg.Id, arg.Name);

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
                if (arg is not SocketUserMessage message || message.Author.IsBot || string.IsNullOrWhiteSpace(message.Content))
                {
                    return;
                }

                Context = new SocketCommandContext(Client, message);

                int argPos = 0;

                if (message.HasStringPrefix(configUtility.CommandDelimiter, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || Context.IsPrivate)
                {
                    IResult result = await Commands.ExecuteAsync(Context, argPos, ServiceProvider);

                    if (message.Channel is SocketDMChannel)
                    {
                        logger.LogInformation("{0}: {1}", message.Author.Username, message.Content);
                    }

                    if (!result.IsSuccess && !string.IsNullOrEmpty(result.ErrorReason))
                    {
                        logger.LogError(result.ErrorReason);
                        await Context.Channel.SendMessageAsync(result.ErrorReason);
                    }

                    logger.LogInformation("Message recieved from {0} ({1}): \"{2}\"", Context.Message.Author.Username, Context.Guild?.Name, Context.Message.Content);
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
                logger.LogError(e, "Error handling the command: {0}", e.Message);
            }
        }

        private Task Client_Connected()
        {
            logger.LogInformation("Connected as {0}", Client.CurrentUser.Username);
            return Task.CompletedTask;
        }

        private async Task Client_Disconnected(Exception arg)
        {
            logger.LogError(arg, "Client disconnected: {0}", arg.Message);

            await Client.LogoutAsync();
            await Client.StopAsync();

            Client.Dispose();

            ShutdownQuartz();

            await ConnectAsync();
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;

            logger.LogInformation("User {0} joined {1}", user.Username, guild.Name);

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

            logger.LogInformation(message);

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

            logger.LogInformation(message);

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
