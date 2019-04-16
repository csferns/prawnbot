using CsvHelper;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using prawnbot_core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public class Helpers
    {
        private SocketCommandContext Context;

        public Helpers(SocketCommandContext _context)
        {
            Context = _context;
        }

        public async Task<IEnumerable<IMessage>> GetAllMessages(ulong id = 453899130486521859)
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

        public async Task WriteToCSV(List<CSVColumns> columns, string folderPath, DateTime startTime, ulong? id = null)
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

        public async Task<List<CSVColumns>> CreateCSVList(ulong id)
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
    }

    public class VoiceRegionTypeReader : TypeReader
    {
        public async override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var region = await context.Guild.GetVoiceRegionsAsync().ToAsyncEnumerable().Flatten().FirstOrDefault();
            return TypeReaderResult.FromSuccess(region);
        }
    }

    public static class ExtensionMethods
    {
        public static bool ContainsSingleLower(this SocketUserMessage message, string textToFind)
        {
            string[] splitMessage = message.Content.ToLowerInvariant().Split(' ');

            foreach (var word in splitMessage)
            {
                if (word.Contains('?'))
                {
                    word.Replace('?', ' ');
                }
            }

            textToFind = textToFind.ToLowerInvariant();

            foreach (var word in splitMessage)
            {
                if (word == textToFind)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsManyLower(this SocketUserMessage message, string textToFind)
        {
            string[] splitMessage = message.Content.ToLowerInvariant().Split(' ');
            string[] splitTextToFind = textToFind.ToLowerInvariant().Split(' ');

            int foundWords = 0;

            foreach (var item in splitTextToFind)
            {
                if (splitMessage.Contains(item)) foundWords++;
            }

            return foundWords == splitTextToFind.Count();
        }

        public static bool IsUserTagged(this SocketUserMessage message, ulong id)
        {
            return message.Content.Contains($"<@!{id}>");
        }
    }

    public static class ContainsCommands
    {
        public static async Task ContainsText(SocketCommandContext context, SocketUserMessage message)
        {
            Random random = new Random();
            Functions functions = new Functions();

            List<string> randomWheel = new List<string>
            {
                "The wheels on the bus go round and round",
                "Excuse me sir, you can't have wheels in this area"
            };

            if (message.ContainsSingleLower("wheel") || message.ContainsSingleLower("bus")) await context.Channel.SendMessageAsync(randomWheel[random.Next(randomWheel.Count())]);
            if (message.ContainsSingleLower("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (message.ContainsSingleLower("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");

            if (message.ContainsManyLower("what can i say")) await context.Channel.SendMessageAsync("except, you're welcome!");
            if (message.ContainsSingleLower("!skip")) await context.Channel.SendMessageAsync("you fucking what");
            if (message.ContainsSingleLower("africa")) await context.Channel.SendMessageAsync("toto by africa");

            if (message.ContainsManyLower("oi oi")) await context.Channel.SendMessageAsync("big boi");
            if (message.ContainsSingleLower("kowalski") || message.ContainsSingleLower("analysis")) await context.Channel.SendMessageAsync("<@!147860921488900097> analysis");
            if (message.ContainsSingleLower("sex"))
            {
                var gifs = await functions.GetGif("calendar");
                await context.Channel.SendMessageAsync($"Have you put it in the calendar? \n{gifs[random.Next(gifs.Length)].url}");
            }
        }

        public static async Task ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.ContainsSingleLower("sam") || message.IsUserTagged(258627811844030465)) await context.Channel.SendMessageAsync("Has the big gay");
            if (message.ContainsSingleLower("ilja") || message.ContainsSingleLower("ultratwink") || message.IsUserTagged(341940376057282560)) await context.Channel.SendMessageAsync("Has terminal gay");
            if (message.ContainsSingleLower("cam") || message.ContainsSingleLower("cameron") || message.IsUserTagged(216177905712103424)) await context.Channel.SendMessageAsync("*Father Cammy");
        }
    }

    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }
    }
}
