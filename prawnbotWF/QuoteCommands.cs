using CsvHelper;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prawnbotWF
{
    public class CSVColumns
    {
        public ulong QuoteID { get; set; }
        public string Author { get; set; }
        public bool AuthorIsBot { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Attachment { get; set; }
    }

    public class QuoteCommands : ModuleBase<SocketCommandContext>
    {
        private async Task<IEnumerable<IMessage>> GetAllQuotes(ulong id = 453899130486521859)
        {
            RequestOptions options = new RequestOptions
            {
                Timeout = 2000,
                RetryMode = RetryMode.RetryTimeouts,
                CancelToken = CancellationToken.None
            };

            IEnumerable<IMessage> messages = await Context.Guild.GetTextChannel(id).GetMessagesAsync(limit: 500000, options: options).FlattenAsync();

            return messages.Reverse();
        }

        [Command("backup")]
        public async Task BackupQuotesAsync(ulong id = 453899130486521859)
        {
            //if (Context.Message.Author.Id != 216177905712103424 || Context.Message.Author.Id != 466926694431719424) return;

            DateTime startTime = DateTime.Now;
            await Context.Channel.SendMessageAsync($"Started backup of channel {Context.Guild.GetChannel(id)} at {startTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

            IEnumerable<IMessage> QuoteRoom = await GetAllQuotes(id);

            List<CSVColumns> records = new List<CSVColumns>();

            for (int i = 0; i < QuoteRoom.Count(); i++)
            {
                if (QuoteRoom.ElementAt(i).Type == MessageType.ChannelPinnedMessage)
                {
                    continue;
                }

                CSVColumns recordToAdd = new CSVColumns
                {
                    QuoteID = QuoteRoom.ElementAt(i).Id,
                    Author = QuoteRoom.ElementAt(i).Author.Username,
                    AuthorIsBot = QuoteRoom.ElementAt(i).Author.IsBot,
                    MessageContent = QuoteRoom.ElementAt(i).Content,
                    Timestamp = QuoteRoom.ElementAt(i).Timestamp
                };

                if (QuoteRoom.ElementAt(i).Attachments.Count() > 0)
                {
                    recordToAdd.Attachment = QuoteRoom.ElementAt(i).Attachments.First().Url;
                }

                records.Add(recordToAdd);
            }

            if (!Directory.Exists($"{Environment.CurrentDirectory}\\ChannelBackups"))
            {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\ChannelBackups");
            }

            string fileName = $"{Context.Guild.GetChannel(id).Name}-backup.csv";
            string filePath = $"{Environment.CurrentDirectory}\\ChannelBackups\\{fileName}";

            using (StreamWriter writer = new StreamWriter(filePath))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                csv.WriteRecords(records);
            }

            TimeSpan completionTime = DateTime.Now - startTime;

            await Context.Channel.SendMessageAsync($"Backed up {QuoteRoom.Count()} messages to {fileName}. The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");
        }

        [Command("addquote")]
        public async Task AddQuoteAsync(string author = "", string year = "", [Remainder]string quote = "")
        {
            //ulong id = 453899130486521859;
            ulong id = 555145972376928257;

            await Context.Guild.GetTextChannel(id).SendMessageAsync($"\"{quote}\" - {author} {year}");

            string filePath = $"{Environment.CurrentDirectory}\\ChannelBackups\\{Context.Guild.GetChannel(id).Name}-backup.csv";

            if (File.Exists(filePath))
            {
                CSVColumns columns = new CSVColumns
                {
                    QuoteID = Context.Message.Id,
                    Author = author,
                    MessageContent = quote,
                    Timestamp = Context.Message.Timestamp
                };

                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fileStream))
                using (CsvWriter csv = new CsvWriter(writer))
                {
                    csv.WriteRecord(columns);
                }
            }

            if (Context.Message.Channel.Id == id) await Context.Message.DeleteAsync();
        }

        [Command("randomquote")]
        public async Task RandomQuoteAsync(ulong id = 453899130486521859)
        {
            if (Context.Message.Author.Id != 258627811844030465)
            {
                IEnumerable<IMessage> QuoteRoom = await GetAllQuotes(id);
                
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
    }
}
