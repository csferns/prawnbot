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
        public ulong MessageID { get; set; }
        public string Author { get; set; }
        public bool AuthorIsBot { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Attachment { get; set; }
    }

    public class QuoteCommands : ModuleBase<SocketCommandContext>
    {
        private async Task<IEnumerable<IMessage>> GetAllMessages(ulong id = 453899130486521859)
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

        private async Task WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id = null)
        {
            string fileName;
            if (id != null) fileName = $"{Context.Guild.GetChannel(id.GetValueOrDefault(0)).Name}-backup.csv";
            else fileName = $"{Context.Guild.Name}-backup.csv";

            string filePath = $"{Environment.CurrentDirectory}\\{folderPath}";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath += $"\\{fileName}";

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecords(columns);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"Backed up {columns.Count()} messages to {fileName}.");

            TimeSpan completionTime = DateTime.Now - startTime;

            sb.Append($"The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");

            await Context.Channel.SendMessageAsync(sb.ToString());            
        }

        private async Task<List<CSVColumns>> CreateCSVList(ulong id)
        {
            List<CSVColumns> records = new List<CSVColumns>();

            IEnumerable<IMessage> messagesToAdd = await GetAllMessages(id);

            for (int i = 0; i < messagesToAdd.Count(); i++)
            {
                if (messagesToAdd.ElementAt(i).Type == MessageType.ChannelPinnedMessage)
                {
                    continue;
                }

                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = messagesToAdd.ElementAt(i).Id,
                    Author = messagesToAdd.ElementAt(i).Author.Username,
                    AuthorIsBot = messagesToAdd.ElementAt(i).Author.IsBot,
                    MessageContent = messagesToAdd.ElementAt(i).Content,
                    Timestamp = messagesToAdd.ElementAt(i).Timestamp
                };

                if (messagesToAdd.ElementAt(i).Attachments.Count() > 0)
                {
                    recordToAdd.Attachment = messagesToAdd.ElementAt(i).Attachments.FirstOrDefault().Url;
                }

                records.Add(recordToAdd);
            }

            return records;
        }

        [Command("backup")]
        public async Task BackupQuotesAsync(ulong id = 453899130486521859)
        {
            if (Context.Message.Author.Id != 216177905712103424 || Context.Message.Author.Id != 466926694431719424) return;

            DateTime startTime = DateTime.Now;
            await Context.Channel.SendMessageAsync($"Started backup of channel {Context.Guild.GetChannel(id)} at {startTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

            var records = await CreateCSVList(id);
            await WriteToCSV(records, "ChannelBackups", startTime, id);
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

                await WriteToCSV(columns, "ChannelBackups", DateTime.Now, id);
            }

            if (Context.Message.Channel.Id == id) await Context.Message.DeleteAsync();
        }

        [Command("randomquote")]
        public async Task RandomQuoteAsync(ulong id = 453899130486521859)
        {
            if (Context.Message.Author.Id != 258627811844030465)
            {
                IEnumerable<IMessage> QuoteRoom = await GetAllMessages(id);
                
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

                var records = await CreateCSVList(textChannel.Id);
                await WriteToCSV(records, $"ServerBackups\\{Context.Guild.Name}", startTime, textChannel.Id);
            }

            TimeSpan completionTime = DateTime.Now - operationTime;
            await Context.Channel.SendMessageAsync($"Finished server backup of {Context.Guild.Name}. The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");
        }
    }
}
