using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        public DiscordBot()
        {
            InitializeComponent();
            
            #region Disabled form components
            disconnectbtn.Enabled = false;

            rpbutton.Enabled = false;

            rptextbox.Enabled = false;
            rptextbox2.Enabled = false;
            rptextbox3.Enabled = false;
            rptextbox4.Enabled = false;

            multirp.Enabled = false;
            rpdropdown.Enabled = false;
            rpdropdown2.Enabled = false;
            rpdropdown3.Enabled = false;
            rpdropdown4.Enabled = false;

            rpdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown2.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown3.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown4.DropDownStyle = ComboBoxStyle.DropDownList;

            streamurl.Enabled = false;
            #endregion

            rpdropdown.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown2.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown3.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown4.DataSource = Enum.GetValues(typeof(ActivityType));
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
                    await Client.StartAsync();
                    await Client.SetStatusAsync(UserStatus.Idle);

                    token_tb.Enabled = false;

                    rpbutton.Enabled = true;
                    rptextbox.Enabled = true;
                    rpdropdown.Enabled = true;
                    multirp.Enabled = true;
                    streamurl.Enabled = true;

                    connect_btn.Enabled = false;
                    disconnectbtn.Enabled = true;
                }
                catch (Exception Ex)
                {
                    await Client_Log(new LogMessage(LogSeverity.Error, "Connecting the bot", $"Error occured while connecting to the bot: \n{Ex.Message}"));
                }

                await Task.Delay(3000);
            }
            else
            {
                await Client_Log(new LogMessage(LogSeverity.Info, "Connecting the bot", "No token given"));
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
                while (multirp.Checked)
                {
                    await Client.SetGameAsync(rptextbox.Text, streamurl.Text, (ActivityType)rpdropdown.SelectedItem);
                    Thread.Sleep(3000);
                    await Client.SetGameAsync(rptextbox2.Text, streamurl.Text, (ActivityType)rpdropdown2.SelectedItem);
                    Thread.Sleep(3000);
                    await Client.SetGameAsync(rptextbox3.Text, streamurl.Text, (ActivityType)rpdropdown3.SelectedItem);
                    Thread.Sleep(3000);
                    await Client.SetGameAsync(rptextbox4.Text, streamurl.Text, (ActivityType)rpdropdown4.SelectedItem);
                    Thread.Sleep(3000);
                }

                await Client.SetGameAsync(rptextbox.Text, streamurl.Text, (ActivityType)rpdropdown.SelectedItem);
            }
            catch (Exception Ex)
            {
                await Client_Log(new LogMessage(LogSeverity.Error, "Rich Presence", $"Error occured while updating the bot's rich presence: \n{Ex.Message}"));
            }
        }

        private async void disconnectbtn_Click(object sender, EventArgs e)
        {
            await Client.LogoutAsync();

            rptextbox.Enabled = false;
            rpdropdown.Enabled = false;

            if (multirp.Checked)
            {
                rptextbox2.Enabled = false;
                rptextbox3.Enabled = false;
                rptextbox4.Enabled = false;

                rpdropdown2.Enabled = false;
                rpdropdown3.Enabled = false;
                rpdropdown4.Enabled = false;

                multirp.Checked = false;
            }

            multirp.Enabled = false;

            connect_btn.Enabled = true;
            token_tb.Enabled = true;
        }

        private void multirp_CheckedChanged(object sender, EventArgs e)
        {
            bool checkedStatus = multirp.Checked;

            rptextbox2.Enabled = checkedStatus;
            rptextbox3.Enabled = checkedStatus;
            rptextbox4.Enabled = checkedStatus;

            rptextbox2.Clear();
            rptextbox3.Clear();
            rptextbox4.Clear();

            rpdropdown2.Enabled = checkedStatus;
            rpdropdown3.Enabled = checkedStatus;
            rpdropdown4.Enabled = checkedStatus;
        }
    }
}
