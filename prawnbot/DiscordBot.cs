using Discord;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prawnbot
{
    public partial class DiscordBot : Form
    {
        prawnbot_core.Functions functions;

        /// <summary>
        /// Creates a new instance of the DiscordBot Form
        /// </summary>
        public DiscordBot()
        {
            functions = new prawnbot_core.Functions();
            InitializeComponent();

            token_tb.UseSystemPasswordChar = false;
            sendToolStripMenuItem.Enabled = false;
            richPresenceToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;
        }

        #region Private methods
        /// <summary>
        /// Method that connects the bot
        /// </summary>
        private async void ConnectBot()
        {
            try
            {
                bool connected = await functions.ConnectAsync(token_tb.Text);

                token_tb.Enabled = false;
                token_tb.UseSystemPasswordChar = true;

                richPresenceToolStripMenuItem.Enabled = true;
                sendToolStripMenuItem.Enabled = true;
                disconnectToolStripMenuItem.Enabled = true;
                tokenconnectbutton.Enabled = false;

                await Task.Delay(-1);

            }
            catch (Exception Ex)
            {
                await functions.PopulateEventLog(new LogMessage(LogSeverity.Error, "Connecting the bot", $"Error occured while connecting to the bot: \n{Ex.Message}"));
            }


        }
        #endregion

        #region Event listeners
        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Message sendMessageForm = new Message();
            sendMessageForm.ShowDialog();
        }

        private void richPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichPresence richPresenceForm = new RichPresence();

            richPresenceForm.ShowDialog();
        }

        /// <summary>
        /// Event listener that watches the connect button for a button click and connects the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tokenconnectbutton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(token_tb.Text))
            {
                ConnectBot();
            }
            else
            {
                await functions.PopulateEventLog(new LogMessage(LogSeverity.Info, "Connecting the bot", "No token given"));
            }
        }

        private async void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await functions.DisconnectAsync();

            token_tb.Enabled = true;
            token_tb.UseSystemPasswordChar = false;

            disconnectToolStripMenuItem.Enabled = false;
            sendToolStripMenuItem.Enabled = false;
            richPresenceToolStripMenuItem.Enabled = false;

            tokenconnectbutton.Enabled = true;
        }
        #endregion

        private async void DiscordBot_FormClosed(object sender, FormClosedEventArgs e)
        {
            await functions.DisconnectAsync();
        }
    }
}
