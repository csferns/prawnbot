using Discord;
using Discord.Commands;
using Discord.WebSocket;
using prawnbot_core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prawnbot
{
    public partial class BotLog : Form
    {
        DiscordSocketClient Client;
        CommandService Commands;
        IServiceProvider Services;

        public BotLog(DiscordSocketClient _client, CommandService _command, IServiceProvider _services)
        {
            InitializeComponent();

            Client = _client;
            Commands = _command;
            Services = _services;
        }

        public async Task PopulateMessageBox(LogMessage arg)
        {
            Invoke((Action)delegate
            {
                messagelog.AppendText(arg.ToString(timestampKind: DateTimeKind.Local) + "\n");
            });

            Task scroll = new Task(new Action(MessageBoxScroll));
            scroll.Start();

            await Task.CompletedTask;
        }

        public async Task PopulateEventBox(LogMessage arg)
        {
            Invoke((Action)delegate
            {
                eventlog.AppendText(arg.ToString(timestampKind: DateTimeKind.Local) + "\n");
            });

            Task scroll = new Task(new Action(EventBoxScroll));
            scroll.Start();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Method to scroll the console output after text has been appended to it
        /// </summary>
        private void EventBoxScroll()
        {
            Invoke(new MethodInvoker(delegate
            {
                eventlog.SelectionStart = eventlog.Text.Length;
                eventlog.ScrollToCaret();
            }));
        }

        /// <summary>
        /// Method to scroll the console output after text has been appended to it
        /// </summary>
        private void MessageBoxScroll()
        {
            Invoke(new MethodInvoker(delegate
            {
                messagelog.SelectionStart = messagelog.Text.Length;
                messagelog.ScrollToCaret();
            }));
        }
    }
}
