using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prawnbotWF
{
    public partial class RichPresence : Form
    {
        DiscordSocketClient Client;

        public RichPresence(DiscordSocketClient _client)
        {
            Client = _client;

            InitializeComponent();

            #region Rich presence dropdowns
            defaultrpdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown1.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown2.DropDownStyle = ComboBoxStyle.DropDownList;
            rpdropdown3.DropDownStyle = ComboBoxStyle.DropDownList;

            defaultrpdropdown.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown1.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown2.DataSource = Enum.GetValues(typeof(ActivityType));
            rpdropdown3.DataSource = Enum.GetValues(typeof(ActivityType));

            defaultrpdropdown.SelectedIndex = 0;
            #endregion

            statusdropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            statusdropdown.DataSource = Enum.GetValues(typeof(UserStatus));
            statusdropdown.SelectedIndex = 1;

            delaytime.Value = 2;
            delaytime.Minimum = 1;
            delaytime.Maximum = 10;

            rptextbox1.Enabled = false;
            rptextbox2.Enabled = false;
            rptextbox3.Enabled = false;

            rpdropdown1.Enabled = false;
            rpdropdown2.Enabled = false;
            rpdropdown3.Enabled = false;
        }

        /// <summary>
        /// Event listener that watches the multiple rich presence checkbox and enables the relevant options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multirpcheckbox_CheckedChanged(object sender, EventArgs e)
        {
            bool checkedStatus = multirpcheckbox.Checked;

            rptextbox1.Enabled = checkedStatus;
            rptextbox2.Enabled = checkedStatus;
            rptextbox3.Enabled = checkedStatus;

            rptextbox1.Clear();
            rptextbox2.Clear();
            rptextbox3.Clear();

            rpdropdown1.Enabled = checkedStatus;
            rpdropdown2.Enabled = checkedStatus;
            rpdropdown3.Enabled = checkedStatus;

            delaytime.Enabled = checkedStatus;
        }

        /// <summary>
        /// Event listener that watches the status button for a button click and updates the status of the bot from the dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void updatestatusbutton_Click(object sender, EventArgs e)
        {
            await Client.SetStatusAsync((UserStatus)statusdropdown.SelectedItem);
        }

        /// <summary>
        /// Event listener that watches the rich presence button for a button click and updates the rich presence of the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void updaterpbutton_Click(object sender, EventArgs e)
        {
            int delay = (int)delaytime.Value * 1000;

            try
            {
                List<ComboBox> rpdropdowns = new List<ComboBox>
                {
                    defaultrpdropdown,
                    rpdropdown1,
                    rpdropdown2,
                    rpdropdown3
                };

                List<TextBox> rptextboxes = new List<TextBox>
                {
                    defaultrptextbox,
                    rptextbox1,
                    rptextbox2,
                    rptextbox3
                };

                var rp = rpdropdowns.Zip(rptextboxes, (dd, tb) => new { Dropdown = dd, Textbox = tb });

                while (multirpcheckbox.Checked)
                {
                    foreach (var item in rp)
                    {
                        await Task.Delay(delay);
                        await Client.SetGameAsync(item.Textbox.Text ?? null, (ActivityType)item.Dropdown.SelectedItem == ActivityType.Streaming ? streamurltextbox.Text : null, (ActivityType)item.Dropdown.SelectedItem);
                    }
                }

                await Client.SetGameAsync(defaultrptextbox.Text ?? null, (ActivityType)defaultrpdropdown.SelectedItem == ActivityType.Streaming ? streamurltextbox.Text : null, (ActivityType)defaultrpdropdown.SelectedItem);
            }
            catch (Exception Ex)
            {
                DiscordBot discordBot = new DiscordBot();
                await discordBot.Client_Log(new LogMessage(LogSeverity.Error, "Rich Presence", $"Error occured while updating the bot's rich presence: \n{Ex.Message}"));
            }

            MessageBox.Show("Rich presence updated successfully!", "Success");
        }
    }
}
