using Discord;
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

        public string[] richpresencepresets = new string[]
        {
            "Listening to",
            "Playing",
            "Watching",
            "Streaming",
        };

        public DiscordBot()
        {
            InitializeComponent();

            #region Disabled form components
            rpbutton.Enabled = false;
            rpdropdown.Enabled = false;
            rptextbox.Enabled = false;
            disconnectbtn.Enabled = false;
            #endregion

            rpdropdown.Items.AddRange(richpresencepresets);
            rpdropdown.SelectedIndex = rpdropdown.FindString("Listening to");
        }

        private async void connect_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(token_tb.Text))
            {
                Client = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Verbose
                });
                Commands = new CommandService();
                Services = new ServiceCollection()
                    .AddSingleton(Client)
                    .AddSingleton(Commands)
                    .BuildServiceProvider();

                Client.Log += Client_Log;
                Client.UserJoined += AnnounceUserJoined;
                Client.UserBanned += AnnounceUserBan;
                Client.MessageReceived += HandleCommandAsync;

                try
                {
                    await RegisterCommandsAsync();

                    await Client.LoginAsync(TokenType.Bot, token_tb.Text);
                    token_tb.Enabled = false;
                    await Client.StartAsync();

                    rpbutton.Enabled = true;
                    rpdropdown.Enabled = true;
                    rptextbox.Enabled = true;
                    connect_btn.Enabled = false;
                    disconnectbtn.Enabled = true;
                }
                catch (Exception Ex)
                {
                    consoleoutput.AppendText($"Error occured while connecting to the bot: \n{Ex.Message}\n");
                }

                await Task.Delay(3000);
            }
            else
            {
                consoleoutput.AppendText("No Token provided! \n");
            }
        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            SocketTextChannel channel = guild.DefaultChannel;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Welcome")
                .WithColor(Color.Blue)
                .WithDescription($"welcome to {guild.Name}, {user.Username}! The server now has {guild.MemberCount} members");

            await channel.SendMessageAsync("", false, builder.Build());
        }

        private async Task AnnounceUserBan(SocketUser user, SocketGuild guild)
        {
            SocketTextChannel channel = guild.DefaultChannel;

            await channel.SendMessageAsync($"{user.Username} was banned.");
        }

        private Task Client_Log(LogMessage arg)
        {
            Invoke((Action)delegate
            {
                consoleoutput.AppendText(arg + "\n");
            });

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("p!", ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                SocketCommandContext context = new SocketCommandContext(Client, message);

                IResult result = await Commands.ExecuteAsync(context, argPos, Services);
            }
        }

        private async void rpbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (richpresencepresets.Contains(rpdropdown.Text) && !string.IsNullOrWhiteSpace(rpdropdown.Text))
                {
                    switch (rpdropdown.Text)
                    {
                        case "Listening to":
                            await Client.SetGameAsync(rptextbox.Text, null, ActivityType.Listening);
                            break;
                        case "Playing":
                            await Client.SetGameAsync(rptextbox.Text, null, ActivityType.Playing);
                            break;
                        case "Watching":
                            await Client.SetGameAsync(rptextbox.Text, null, ActivityType.Watching);
                            break;
                        case "Streaming":
                            await Client.SetGameAsync(rptextbox.Text, null, ActivityType.Streaming);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                consoleoutput.AppendText($"Error occured while updating the bot's rich presence: \n{Ex.Message}\n");
                throw;
            }
        }

        private async void disconnectbtn_Click(object sender, EventArgs e)
        {
            await Client.LogoutAsync();
        }
    }
}
