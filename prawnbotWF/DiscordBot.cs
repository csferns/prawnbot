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
            DisableComponents();
            DropdownComponents();

            delaytime.Value = 2;
            delaytime.Minimum = 1;
            delaytime.Maximum = 10;

            token_tb.UseSystemPasswordChar = false;
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
        private Task Client_Log(LogMessage arg)
        {
            Invoke((Action)delegate
            {
                consoleoutput.AppendText(arg.ToString(timestampKind: DateTimeKind.Utc) + "\n");
            });

            Task scroll = new Task(new Action(ConsoleScroll));
            scroll.Start();

            return Task.CompletedTask;
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
        /// <param name="arg"></param>
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
        /// Disable form components on startup that need to be disabled
        /// </summary>
        private void DisableComponents()
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

        /// <summary>
        /// Set values of dropdowns on startup
        /// </summary>
        private void DropdownComponents()
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

            rpdropdown.SelectedIndex = 0;
            #endregion

            statusdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            statusdropdown.DataSource = Enum.GetValues(typeof(UserStatus));
            statusdropdown.SelectedIndex = 1;
        }

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
                await Client.SetStatusAsync((UserStatus)statusdropdown.SelectedItem);

                statusdropdown.Enabled = true;
                statusbutton.Enabled = true;

                token_tb.Enabled = false;
                token_tb.UseSystemPasswordChar = true;

                rpbutton.Enabled = true;
                rptextbox.Enabled = true;
                rpdropdown.Enabled = true;
                multirp.Enabled = true;
                streamurl.Enabled = true;

                connect_btn.Enabled = false;
                disconnectbtn.Enabled = true;

                await Task.Delay(3000);
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
            if (message.Content.Contains("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (message.Content.Contains("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");
            if (message.Content.Contains("kowalski") || message.Content.Contains("analysis")) await context.Channel.SendMessageAsync($"<@!147860921488900097> analysis");
        }
        #endregion

        #region Event listeners
        /// <summary>
        /// Event listener that watches the connect button for a button click and connects the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void connect_btn_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Event listener that watches the rich presence button for a button click and updates the rich presence of the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        await Task.Delay(delay);
                        await Client.SetGameAsync(item.Textbox.Text ?? null, (ActivityType)item.Dropdown.SelectedItem == ActivityType.Streaming ? streamurl.Text : null, (ActivityType)item.Dropdown.SelectedItem);
                    }
                }

                await Client.SetGameAsync(rptextbox.Text ?? null, (ActivityType)rpdropdown.SelectedItem == ActivityType.Streaming ? streamurl.Text : null, (ActivityType)rpdropdown.SelectedItem);
            }
            catch (Exception Ex)
            {
                await Client_Log(new LogMessage(LogSeverity.Error, "Rich Presence", $"Error occured while updating the bot's rich presence: \n{Ex.Message}"));
            }

            MessageBox.Show("Rich presence updated successfully!", "Success");
        }

        /// <summary>
        /// Event listener that watches the disconnect button for a button click and disconnects the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void disconnectbtn_Click(object sender, EventArgs e)
        {
            await Client.LogoutAsync();

            DisableComponents();

            connect_btn.Enabled = true;
            token_tb.Enabled = true;
            token_tb.UseSystemPasswordChar = false;

            ActiveForm.Text = $"Discord Bot";
        }

        /// <summary>
        /// Event listener that watches the multiple rich presence checkbox and enables the relevant options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event listener that watches the status button for a button click and updates the status of the bot from the dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Statusbutton_Click(object sender, EventArgs e)
        {
            await Client.SetStatusAsync((UserStatus)statusdropdown.SelectedItem);
        }
        #endregion
    }
}
