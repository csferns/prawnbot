using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = Discord.Color;

namespace prawnbotWF
{
    public partial class DiscordBot : Form
    {
        DiscordSocketClient Client;
        CommandService Commands;
        IServiceProvider Services;
        AudioService Audio;

        /// <summary>
        /// Creates a new instance of the DiscordBot Form
        /// </summary>
        public DiscordBot()
        {
            InitializeComponent();

            token_tb.UseSystemPasswordChar = false;
            sendToolStripMenuItem.Enabled = false;
            richPresenceToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;
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
            SocketTextChannel channel = guild.DefaultChannel;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Banned.")
                .WithColor(Color.Red)
                .WithDescription($"{user.Username} was banned.")
                .WithCurrentTimestamp();

            await channel.SendMessageAsync("", false, builder.Build());
        }

        /// <summary>
        /// Log to the console events that have happened
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task Client_Log(LogMessage arg)
        {
            Invoke((Action)delegate
            {
                consoleoutput.AppendText(arg.ToString(timestampKind: DateTimeKind.Utc) + "\n");
            });

            Task scroll = new Task(new Action(ConsoleScroll));
            scroll.Start();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Method to scroll the console output after text has been appended to it
        /// </summary>
        private void ConsoleScroll()
        {
            Invoke(new MethodInvoker(delegate
            {
                consoleoutput.SelectionStart = consoleoutput.Text.Length;
                consoleoutput.ScrollToCaret();
            }));
        }
        
        /// <summary>
        /// Event handler to handle the messages that come in
        /// </summary>
        /// <param name="arg">Message to pass into the commands</param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            SocketCommandContext context = new SocketCommandContext(Client, message);

            int argPos = 0;

            ContainsCommands(context, message);

            if (message.HasStringPrefix("p!", ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                if (context.Guild != null)
                {
                    IResult result = await Commands.ExecuteAsync(context, argPos, Services);

                    if (!result.IsSuccess) await Client_Log(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));
                }
                else
                {
                    await context.Channel.SendMessageAsync("No channel available!");
                }
            }
        }

        #region Private methods
        /// <summary>
        /// Method that connects the bot
        /// </summary>
        private async void ConnectBot()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            Commands = new CommandService();
            Audio = new AudioService();
            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .AddSingleton(Audio)
                .BuildServiceProvider();

            Client.Log += Client_Log;
            Client.UserJoined += AnnounceUserJoined;
            Client.UserBanned += AnnounceUserBan;
            Client.MessageReceived += HandleCommandAsync;

            try
            {
                await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);

                await Client.LoginAsync(TokenType.Bot, token_tb.Text);
                await Client.StartAsync();
                await Client.SetStatusAsync(UserStatus.Online);

                token_tb.Enabled = false;
                token_tb.UseSystemPasswordChar = true;

                richPresenceToolStripMenuItem.Enabled = true;
                sendToolStripMenuItem.Enabled = true;
                disconnectToolStripMenuItem.Enabled = true;
                tokenconnectbutton.Enabled = false;

                //await Task.Delay(3000);
                //ActiveForm.Text = $"Discord Bot - {Client.CurrentUser.Username}";
            }
            catch (Exception Ex)
            {
                await Client_Log(new LogMessage(LogSeverity.Error, "Connecting the bot", $"Error occured while connecting to the bot: \n{Ex.Message}"));
            }

            await Task.Delay(-1);
        }

        private async void ContainsCommands(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.ContainsLower("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (message.ContainsLower("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");
            if (message.ContainsLower("@here")) await context.Channel.SendMessageAsync("@far, wherever you are");

            if (message.ContainsLower("what can i say")) await context.Channel.SendMessageAsync("except, you're welcome!");
            if (message.ContainsLower("overwatch")) await context.Channel.SendMessageAsync("is gay");
            if (message.ContainsLower("I")) await context.Channel.SendMessageAsync("AYE AYE");

            if (message.ContainsLower("!skip")) await context.Channel.SendMessageAsync("you fucking what");
            if (message.ContainsLower("africa")) await context.Channel.SendMessageAsync("toto by africa");
            if (message.ContainsLower("oi oi")) await context.Channel.SendMessageAsync("big boi");

            if (message.ContainsLower("sam") || message.ContainsLower("<@!258627811844030465>")) await context.Channel.SendMessageAsync("has the big gay");
            if (message.ContainsLower("kowalski") || message.ContainsLower("analysis")) await context.Channel.SendMessageAsync("<@!147860921488900097> analysis");

            List<string> RandomWheel = new List<string>
            {
                "The wheels on the bus go round and round",
                "Excuse me sir, you can't have wheels in this area"
            };

            Random random = new Random();

            if (message.ContainsLower("wheel") || message.ContainsLower("bus")) await context.Channel.SendMessageAsync(RandomWheel[random.Next(RandomWheel.Count())]);
        }
        #endregion

        #region Event listeners
        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Message sendMessageForm = new Message(Client);

            sendMessageForm.ShowDialog();
        }

        private void richPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichPresence richPresenceForm = new RichPresence(Client);

            richPresenceForm.ShowDialog();
        }

        /// <summary>
        /// Event listener that watches the connect button for a button click and connects the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private async void tokenconnectbutton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(token_tb.Text))
            {
                ConnectBot();
            }
            else
            {
                await Client_Log(new LogMessage(LogSeverity.Info, "Connecting the bot", "No token given"));
            }
        }

        private async void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await Client.LogoutAsync();

            token_tb.Enabled = true;
            token_tb.UseSystemPasswordChar = false;

            disconnectToolStripMenuItem.Enabled = false;
            sendToolStripMenuItem.Enabled = false;
            richPresenceToolStripMenuItem.Enabled = false;

            tokenconnectbutton.Enabled = true;
        }
        #endregion
    }
}
