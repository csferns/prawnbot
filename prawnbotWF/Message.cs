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
    public partial class Message : Form
    {
        DiscordSocketClient Client;

        public Message(DiscordSocketClient _client)
        {
            Client = _client;

            InitializeComponent();

            availableGuilds.DataSource = Client.Guilds.ToList();
        }

        private void findTextChannels_Click(object sender, EventArgs e)
        {
            var selectedGuild = Client.Guilds.FirstOrDefault(x => x == availableGuilds.SelectedItem);

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

                MessageBox.Show($"Message sent to {textChannels.SelectedItem}", "Success");

                messageContent.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Message content can't be empty!", "Error");
            }
        }
    }
}
