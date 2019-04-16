using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace prawnbot
{
    public partial class Message : Form
    {
        prawnbot_core.Functions functions;

        public Message()
        {
            functions = new prawnbot_core.Functions();
            InitializeComponent();

            textChannels.Enabled = false;
            availableGuilds.Enabled = false;

            sendButton.Enabled = false;
            findTextChannels.Enabled = false;
            messageContent.Enabled = false;
        }

        private void findTextChannels_Click(object sender, EventArgs e)
        {
            if (availableGuilds.SelectedItem.GetType() == typeof(SocketGuild))
            {
                var selectedGuild = functions.GetGuildTextChannels((SocketGuild)availableGuilds.SelectedItem);
                textChannels.DataSource = selectedGuild.ToList();
                textChannels.SelectedItem = functions.GetDefaultChannel((SocketGuild)availableGuilds.SelectedItem);

                textChannels.Enabled = true;
            }

            sendButton.Enabled = true;

        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageContent.Text))
            {
                if (availableGuilds.SelectedItem.GetType() == typeof(SocketGuild))
                {
                    await functions.SendChannelMessageAsync((SocketGuild)availableGuilds.SelectedItem, (SocketTextChannel)textChannels.SelectedItem, messageContent.Text);
                }
                else
                {
                    await functions.SendDMAsync((SocketGuildUser)availableGuilds.SelectedItem, messageContent.Text);
                }

                MessageBox.Show($"Message sent to {textChannels.SelectedItem}!", "Success");

                messageContent.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Message content can't be empty!", "Error");
            }
        }

        private void availableGuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            textChannels.DataSource = null;
            textChannels.Enabled = false;
        }

        private void guildbutton_Click(object sender, EventArgs e)
        {
            availableGuilds.DataSource = null;
            availableGuilds.DataSource = functions.GetAllGuilds();

            if (!availableGuilds.Enabled) availableGuilds.Enabled = true;
            findTextChannels.Enabled = true;

            guildbutton.Enabled = false;
            if (!dmbutton.Enabled) dmbutton.Enabled = true;
        }

        private void dmbutton_Click(object sender, EventArgs e)
        {
            List<SocketGuildUser> users = functions.GetAllUsers();

            availableGuilds.DataSource = null;
            availableGuilds.DataSource = users.OrderByDescending(x => x.Nickname).Distinct().ToList();

            if (!availableGuilds.Enabled) availableGuilds.Enabled = true;

            sendButton.Enabled = true;
            messageContent.Enabled = true;

            dmbutton.Enabled = false;
            if (!guildbutton.Enabled) guildbutton.Enabled = true;
            if (findTextChannels.Enabled) findTextChannels.Enabled = false;
            messageContent.Enabled = true;
        }

        private void TextChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            messageContent.Enabled = true;
        }
    }
}
