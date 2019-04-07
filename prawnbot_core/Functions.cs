using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Functions
    {
        public Functions()
        {

        }

        DiscordSocketClient _client;
        CommandService _commands;
        IServiceProvider _services;
        AudioService _audio;

        public DiscordSocketClient InstanciateClient()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            return _client;
        }

        public CommandService InstanciateCommands()
        {
            _commands = new CommandService();
            _commands.AddTypeReader(typeof(IVoiceRegion), new VoiceRegionTypeReader());

            return _commands;
        }

        public IServiceProvider InstanciateServices()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_audio)
                .BuildServiceProvider();

            return _services;
        }

        public AudioService InstanciateAudio()
        {
            _audio = new AudioService();

            return _audio;
        }

        public async Task<bool> ConnectAsync(string token)
        {
            InstanciateClient();
            InstanciateCommands();
            InstanciateAudio();
            InstanciateServices();

            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += AnnounceUserJoined;
            _client.UserBanned += AnnounceUserBan;
            _client.Connected += Client_Connected;
            _client.UserUnbanned += Client_UserUnbanned;
            _client.Log += PopulateEventLog;

            await _commands.AddModulesAsync(Assembly.Load(typeof(Commands).Assembly.FullName), _services);

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await SetBotStatusAsync(UserStatus.Online);

            return true;
        }

        public async Task DisconnectAsync()
        {
            if (_client != null) await _client.LogoutAsync();
        }

        /// <summary>
        /// Event handler to handle the messages that come in
        /// </summary>
        /// <param name="arg">Message to pass into the commands</param>
        /// <returns></returns>
        public async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            int argPos = 0;

            ContainsCommands.ContainsText(context, message);
            ContainsCommands.ContainsUser(context, message);

            if (message.HasStringPrefix("p!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) || context.IsPrivate)
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _services);

                if (message.Channel.GetType() == typeof(SocketDMChannel)) await PopulateMessageLog(new LogMessage(LogSeverity.Info, "Message", $"{message.Author.Username}: {message.Content}"));

                if (!result.IsSuccess) await PopulateEventLog(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));

                if (
                    result.ErrorReason != null
                    && result.ErrorReason.ToLowerInvariant().Contains("unknown")
                    && message.Channel.GetType() != typeof(SocketDMChannel)
                   )
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        /// <summary>
        /// Announce to the server a user joined
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task AnnounceUserJoined(SocketGuildUser user)
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
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        private async Task AnnounceUserBan(SocketUser user, SocketGuild guild)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Banned.")
                .WithColor(Color.Red)
                .WithDescription($"{user.Username} was banned from {guild.Name}.")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
        }

        private async Task Client_UserUnbanned(SocketUser user, SocketGuild guild)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Unbanned.")
                .WithColor(Color.Green)
                .WithDescription($"{user.Username} was unbanned from {guild.Name}.")
                .WithCurrentTimestamp();

            await guild.DefaultChannel.SendMessageAsync("", false, builder.Build());
        }

        private async Task Client_Connected()
        {
            while (_client.ConnectionState == ConnectionState.Connected)
            {
                await prawnbot_core.Commands.MemeOClock(_client);
            }
        }

        public async Task SetBotStatusAsync(UserStatus status)
        {
            await _client.SetStatusAsync(status);
        }

        public List<SocketGuild> GetAllGuilds()
        {
            return _client.Guilds.ToList();
        }

        public List<SocketGuildUser> GetAllUsers()
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (var guild in _client.Guilds)
            {
                foreach (var user in guild.Users.Where(x => !x.IsBot))
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public SocketTextChannel GetDefaultChannel(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).DefaultChannel;
        }

        public IReadOnlyCollection<SocketTextChannel> GetGuildTextChannels(SocketGuild guild)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels;
        }

        public SocketTextChannel GetGuildTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            return _client.Guilds.FirstOrDefault(x => x == guild).TextChannels.FirstOrDefault(x => x == channel);
        }
    }
}
