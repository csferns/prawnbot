using CsvHelper;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class CSVColumns
    {
        public ulong MessageID { get; set; }
        public string Author { get; set; }
        public bool AuthorIsBot { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Attachment { get; set; }
    }

    public class Alarm
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
    }

    public static class ExtensionMethods
    {
        public static bool ContainsSingleLower(this SocketUserMessage message, string textToFind)
        {
            string[] splitMessage = message.Content.ToLowerInvariant().Split(' ');
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
        public static async void ContainsText(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.ContainsSingleLower("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (message.ContainsSingleLower("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");
            if (message.ContainsManyLower("what can i say")) await context.Channel.SendMessageAsync("except, you're welcome!");

            if (message.ContainsSingleLower("!skip")) await context.Channel.SendMessageAsync("you fucking what");
            if (message.ContainsSingleLower("africa")) await context.Channel.SendMessageAsync("toto by africa");
            if (message.ContainsManyLower("oi oi")) await context.Channel.SendMessageAsync("big boi");

            if (message.ContainsSingleLower("kowalski") || message.ContainsSingleLower("analysis")) await context.Channel.SendMessageAsync("<@!147860921488900097> analysis");

            List<string> RandomWheel = new List<string>
            {
                "The wheels on the bus go round and round",
                "Excuse me sir, you can't have wheels in this area"
            };

            Random random = new Random();

            if (message.ContainsSingleLower("wheel") || message.ContainsSingleLower("bus")) await context.Channel.SendMessageAsync(RandomWheel[random.Next(RandomWheel.Count())]);
        }

        public static async void ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.ContainsSingleLower("sam") || message.IsUserTagged(258627811844030465)) await context.Channel.SendMessageAsync("has the big gay");
            if (message.ContainsSingleLower("ilja") || message.IsUserTagged(341940376057282560)) await context.Channel.SendMessageAsync("Has terminal gay");
            if (message.ContainsSingleLower("cam") || message.ContainsSingleLower("cameron") || message.IsUserTagged(216177905712103424)) await context.Channel.SendMessageAsync("*Father Cammy");
        }
    }

    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IAudioChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }

            var audioClient = await target.ConnectAsync();
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public async Task CreateStream(Stream stream, string path)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });

            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        }

        /// <summary>
        /// write example PCM data of a note to a stream
        /// </summary>
        public void GeneratePCM(Stream stream, double frequency, double duration, double volume)
        {
            const int channels = 2; // stereo
            const int sampleRate = 48000; // 48000 samples per sec

            // binary writer uses Little Endian, which is what we need
            var writer = new BinaryWriter(stream);
            int sampleCount = (int)Math.Round(duration * sampleRate);
            for (int t = 0; t < sampleCount; t++)
            {
                for (int c = 0; c < channels; c++)
                {
                    // create sample
                    double sampleValue = Math.Sin(2 * Math.PI * (frequency * t / sampleRate)) * volume;
                    // encode sample in 16 bits
                    short sample16bit = (short)Math.Round(short.MaxValue * sampleValue);
                    writer.Write(sample16bit);
                }
            }
        }
    }
}
