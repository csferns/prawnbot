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

        public DiscordBot()
        {
            InitializeComponent();
            DisableComponents();
            DropdownComponents();

            delaytime.Value = 1;
            delaytime.Minimum = -1;
            delaytime.Maximum = 10;
        }

        public void DisableComponents()
        {
            disconnectbtn.Enabled = false;

            rpbutton.Enabled = false;

            rptextbox.Enabled = false;
            rptextbox2.Enabled = false;
            rptextbox3.Enabled = false;
            rptextbox4.Enabled = false;

            rpdropdown.Enabled = false;
            rpdropdown2.Enabled = false;
            rpdropdown3.Enabled = false;
            rpdropdown4.Enabled = false;

            multirp.Checked = false;

            multirp.Enabled = false;

            streamurl.Enabled = false;
            delaytime.Enabled = false;

            statusdropdown.Enabled = false;
            statusbutton.Enabled = false;
        }

        public void DropdownComponents()
        {
            #region Rich presence dropdowns
            rpdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown2.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown3.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown4.DropDownStyle = ComboBoxStyle.DropDownList;

            rpdropdown.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown2.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown3.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown4.DataSource = Enum.GetValues(typeof(ActivityType));
            #endregion

            statusdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            statusdropdown.DataSource = Enum.GetValues(typeof(UserStatus));
            statusdropdown.SelectedIndex = 1;
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
                if (context.Guild != null)
                {
                    IResult result = await Commands.ExecuteAsync(context, argPos, Services);

                    if (!result.IsSuccess)
                        await Client_Log(new LogMessage(LogSeverity.Error, "Commands", result.ErrorReason));
                }
                else
                {
                    await context.Channel.SendMessageAsync("No channel available!");
                }
            }
        }

        #region Event listeners
        private async void connect_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(token_tb.Text))
            {
                Client = new DiscordSocketClient(new DiscordSocketConfig()
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
                    await RegisterCommandsAsync();

                    await Client.LoginAsync(TokenType.Bot, token_tb.Text);
                    await Client.StartAsync();

                    statusdropdown.Enabled = true;
                    statusbutton.Enabled = true;
                    await Client.SetStatusAsync((UserStatus)statusdropdown.SelectedItem);

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

                await Task.Delay(-1);
            }
            else
            {
                await Client_Log(new LogMessage(LogSeverity.Info, "Connecting the bot", "No token given"));
            }
        }

        private async void rpbutton_Click(object sender, EventArgs e)
        {
            int delay = (int)delaytime.Value * 1000;

            try
            {
                List<ComboBox> rpdropdowns = new List<ComboBox>
                {
                    rpdropdown,
                    rpdropdown2,
                    rpdropdown3,
                    rpdropdown4
                };

                List<TextBox> rptextboxes = new List<TextBox>
                {
                    rptextbox,
                    rptextbox2,
                    rptextbox3,
                    rptextbox4
                };

                var rp = rpdropdowns.Zip(rptextboxes, (dd, tb) => new { Dropdown = dd, Textbox = tb });

                while (multirp.Checked)
                {
                    foreach (var item in rp)
                    {
                        await Task.Delay(delay / 2);
                        await Client.SetGameAsync(item.Textbox.Text ?? null, (ActivityType)item.Dropdown.SelectedItem == ActivityType.Streaming ? streamurl.Text : null, (ActivityType)item.Dropdown.SelectedItem);
                        await Task.Delay(delay / 2);
                    }
                }

                await Client.SetGameAsync(rptextbox.Text ?? null, (ActivityType)rpdropdown.SelectedItem == ActivityType.Streaming ? streamurl.Text : null, (ActivityType)rpdropdown.SelectedItem);
            }
            catch (Exception Ex)
            {
                await Client_Log(new LogMessage(LogSeverity.Error, "Rich Presence", $"Error occured while updating the bot's rich presence: \n{Ex.Message}"));
            }
        }

        private async void disconnectbtn_Click(object sender, EventArgs e)
        {
            await Client.LogoutAsync();

            DisableComponents();

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

            delaytime.Enabled = checkedStatus;
        }

        //private async void Statusdropdown_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    
        //}
        #endregion

        private async void Statusbutton_Click(object sender, EventArgs e)
        {
            await Client.SetStatusAsync((UserStatus)statusdropdown.SelectedItem);
        }
    }
}
