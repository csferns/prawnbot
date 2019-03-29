using Discord.WebSocket;
using System;
using System.Linq;
using System.Windows.Forms;

namespace prawnbot
{
    public partial class Message : Form
    {
        DiscordSocketClient Client;

        public Message(DiscordSocketClient _client)
        {
            Client = _client;

            InitializeComponent();

            availableGuilds.DataSource = Client.Guilds.ToList();
            textChannels.Enabled = false;
        }

        private void findTextChannels_Click(object sender, EventArgs e)
        {
            var selectedGuild = Client.Guilds.FirstOrDefault(x => x == availableGuilds.SelectedItem);

            textChannels.Enabled = true;
            textChannels.DataSource = selectedGuild.TextChannels.ToList();
            textChannels.SelectedItem = selectedGuild.DefaultChannel;
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageContent.Text))
            {
                await Client.Guilds.FirstOrDefault(x => x == availableGuilds.SelectedItem)
                    .TextChannels.FirstOrDefault(x => x == textChannels.SelectedItem)
                    .SendMessageAsync(messageContent.Text);

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
    }
}
