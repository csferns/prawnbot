using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("backup")]
        public async Task BackupQuotesAsync(ulong id = 453899130486521859)
        {
            if (Context.Message.Author.Id != 216177905712103424 || Context.Message.Author.Id != 466926694431719424) return;

            DateTime startTime = DateTime.Now;
            await Context.Channel.SendMessageAsync($"Started backup of channel {Context.Guild.GetChannel(id)} at {startTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

            var records = await helpers.CreateCSVList(id);
            await helpers.WriteToCSV(records, "ChannelBackups", startTime, id);
        }

        [Command("addquote")]
        public async Task AddQuoteAsync(string author = "", string year = "", [Remainder]string quote = "")
        {
            ulong id = ulong.MinValue;

            switch (Context.Guild.Name)
            {
                case "c.ferns":
                    id = 555145972376928257;
                    break;
                case "#WalrusForFührer":
                    id = 453899130486521859;
                    break;
                default:
                    break;
            }

            if (quote.Contains("\\n")) quote = quote.Replace("\\n", "\n");

            await Context.Guild.GetTextChannel(id).SendMessageAsync($"\"{quote}\" - {author} {year}");

            string filePath = $"{Environment.CurrentDirectory}\\ChannelBackups\\{Context.Guild.GetChannel(id).Name}-backup.csv";

            List<CSVColumns> columns = new List<CSVColumns>();

            if (File.Exists(filePath))
            {
                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = Context.Message.Id,
                    Author = author,
                    MessageContent = quote,
                    Timestamp = Context.Message.Timestamp
                };

                columns.Add(recordToAdd);

                await helpers.WriteToCSV(columns, "ChannelBackups", DateTime.Now, id);
            }

            if (Context.Message.Channel.Id == id) await Context.Message.DeleteAsync();
        }

        [Command("randomquote")]
        public async Task RandomQuoteAsync(ulong id = 453899130486521859)
        {
            if (Context.Message.Author.Id != 258627811844030465)
            {
                IEnumerable<IMessage> QuoteRoom = await helpers.GetAllMessages(id);
                
                Random random = new Random();

                IMessage randomQuote = QuoteRoom.ElementAt(random.Next(QuoteRoom.Count()));

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithAuthor(randomQuote.Author)
                    .WithColor(Color.Blue)
                    .WithDescription(randomQuote.Content)
                    .WithTimestamp(randomQuote.Timestamp);

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else
            {
                await ReplyAsync("Fuck off Sam");
            }
        }

        [Command("backupserver")]
        public async Task BackupServerAsync()
        {
            DateTime operationTime = DateTime.Now;

            await Context.Channel.SendMessageAsync($"Started backup of server {Context.Guild.Name} ({Context.Guild.TextChannels.Count()} channels) at {operationTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

            foreach (var textChannel in Context.Guild.TextChannels)
            {
                DateTime startTime = DateTime.Now;

                var records = await helpers.CreateCSVList(textChannel.Id);
                await helpers.WriteToCSV(records, $"ServerBackups\\{Context.Guild.Name}", startTime, textChannel.Id);
            }

            TimeSpan completionTime = DateTime.Now - operationTime;
            await Context.Channel.SendMessageAsync($"Finished server backup of {Context.Guild.Name}. The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");
        }
    }
}
