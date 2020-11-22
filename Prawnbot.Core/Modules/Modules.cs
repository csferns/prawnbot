using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Prawnbot.Common;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Attributes;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Exceptions;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prawnbot.Core.Modules
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        private readonly IBotService botService;
        private readonly ICoreService coreService;
        private readonly IFileService fileService;
        private readonly IAPIService apiService;
        private readonly IAlarmService alarmService;
        private readonly ISpeechRecognitionService speechRecognitionService;

        public Modules(IBotService botService, ICoreService coreService, IFileService fileService, IAPIService apiService, ISpeechRecognitionService speechRecognitionService, IAlarmService alarmService)
        {
            this.botService = botService;
            this.coreService = coreService;
            this.fileService = fileService;
            this.apiService = apiService;
            this.alarmService = alarmService;
            this.speechRecognitionService = speechRecognitionService;
        }

        #if DEBUG
        [Command("exception-test")]
        [NotImplemented]
        public async Task ExceptionTester()
        {
            await Task.Delay(100);
            throw new UnexpectedBananaException();
        }
        #endif

        [Command("order-message")]
        public async Task OrderMessageAsync(bool includeSpaces = true)
        {
            ListResponse<IMessage> messages = await coreService.GetAllMessagesAsync(Context.Channel.Id, 100);
            IMessage message = messages.Entities.ToList()[messages.Entities.Count() - 2];

            StringBuilder sb = new StringBuilder();

            if (includeSpaces)
            {
                string[] splitMessage = message.Content.Split(' ');

                for (int item = 0; item < splitMessage.Count(); item++)
                {
                    HashSet<char> orderedMessage = splitMessage[item].OrderBy(x => x).ToHashSet();

                    for (int character = 0; character < orderedMessage.Count(); character++)
                    {
                        sb.Append(orderedMessage.ElementAt(character));
                    }

                    if (item != splitMessage.Count())
                    {
                        sb.Append(' ');
                    }
                }
            }
            else
            {
                IEnumerable<char> orderedMessage = message.Content.RemoveSpecialCharacters().ToLowerInvariant().Where(x => x != ' ').OrderBy(x => x);

                foreach (char character in orderedMessage)
                {
                    sb.Append(character);
                }
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("random-user")]
        [Summary("Gets a random user from the guild and posts them")]
        public async Task RandomUserAsync()
        {
            SocketGuildUser randomUser = coreService.GetAllUsers().Entities.RandomOrDefault();

            await Context.Channel.SendMessageAsync(randomUser.Nickname ?? randomUser.Username);
        }

        [Command("bot-ping")]
        [Summary("Returns the connection latency of the bot to the Discord servers")]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync($"{Context.Client.Latency}ms");
        }

        [Command("commands")]
        [Summary("PM's a list of commands to the user")]
        public async Task CommandsAsync(bool includeNotImplemented = false)
        {
            await coreService.CommandsAsync(includeNotImplemented);
        }

        [Command("bot-info")]
        [Summary("Gets the bot's information")]
        public async Task GetBotInfoAsync()
        {
            await coreService.GetBotInfoAsync();
        }

        [Command("server-status")]
        [Summary("Gives the status of the server")]
        public async Task StatusAsync()
        {
            await coreService.StatusAsync();
        }

        [Command("copypasta")]
        [Summary("Gives a copypasta back to the user")]
        public async Task CopypastaAsync([Remainder]string copypastaName)
        {
            string toMessage = "I didn't recognise that";
            string fileName = "..\\copypastas.json";

            if (!string.IsNullOrEmpty(copypastaName) && File.Exists(fileName))
            {
                string file = File.ReadAllText(fileName);

                Dictionary<string, string> copypastas = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);

                if (copypastas.ContainsKey(copypastaName))
                {
                    toMessage = copypastas[copypastaName];
                }         
            }

            await Context.Channel.SendMessageAsync($"{Context.User.Mention} {toMessage}");
        }

        [Command("user-joined")]
        [Summary("Gets when the user joined the server")]
        public async Task GetUserJoinedAsync(SocketGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} joined at {user.JoinedAt}");
        }

        [Command("user-created")]
        [Summary("Gets when the user joined discord")]
        public async Task GetUserCreatedAsync(SocketGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"User {user.Mention} was created on {user.CreatedAt.ToLocalTime()}");
        }

        [Command("current-region")]
        [Summary("Gets the server's current region")]
        public async Task CurrentRegionAsync()
        {
            await Context.Channel.SendMessageAsync($"The server is currently located in: {Format.Bold(Context.Client.VoiceRegions.FirstOrDefault(x => x.Id == Context.Guild.VoiceRegionId).Name)}");
        }

        [Command("all-regions")]
        [Summary("Gets all the server regions")]
        public async Task GetAllRegionsAsync()
        {
            IEnumerable<IVoiceRegion> allRegions = Context.Client.VoiceRegions.Where(x => !x.IsDeprecated && !x.IsVip);
            string currentRegion = Context.Guild.VoiceRegionId;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Format.Underline("Active regions:"));
            foreach (IVoiceRegion region in allRegions)
            {
                bool bold = false;
                string s = string.Empty;

                s += region.Name;

                if (region.IsOptimal)
                {
                    s += " (Optimal)";
                    bold = true;
                }

                if (region.Id == currentRegion)
                {
                    s += " (Current)";
                    bold = true;
                }

                sb.AppendLine(bold ? Format.Bold(s) : s);
            }

            sb.AppendLine(Format.Underline("Deprecated regions:"));
            foreach (IVoiceRegion region in allRegions)
            {
                sb.AppendLine($"{(region.IsOptimal || region.Id == currentRegion ? Format.Bold(region.Name) : region.Name)} {(region.Id == currentRegion ? "(Current)" : "")}");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("set-region")]
        [Summary("Sets the current region of the server")]
        public async Task SetRegionAsync([Remainder]string regionName = null)
        {
            await botService.SetBotRegionAsync(regionName);
        }

        [Command("set-alarm")]
        [Summary("Sets an alarm for a user")]
        [NotImplemented]
        public async Task SetAlarmAsync(int timePassed, [Remainder]string alarmName = null)
        {
            await Task.Delay(2);
            throw new NotImplementedException();
        }

        [Command("display-alarms")]
        [Summary("Display set alarms")]
        public async Task GetAlarmAsync()
        {
            ListResponse<AlarmDTO> response = alarmService.GetAll();
            if (response.HasData)
            {
                HashSet<AlarmDTO> alarms = response.Entities.ToHashSet();
                IEnumerable<string> alarmDataToReturn = alarms.Select(x => $"{x.User}'s alarm {x.AlarmName}: {x.AlarmName}");
                await Context.Channel.SendMessageAsync(string.Join(',', alarmDataToReturn));
            }
            else
            {
                await Context.Channel.SendMessageAsync("No alarms set");
            }
        }

        [Command("remove-alarm")]
        [Summary("Remove a set alarm")]
        [NotImplemented]
        public async Task RemoveAlarmAsync()
        {
            await Task.Delay(200);
            throw new NotImplementedException();
        }

        [Command("backup-channel")]
        [Summary("Backs up a channel with a given ID")]
        public async Task BackupQuotesAsync(ulong? id = null)
        {
            if (Context.Message.Author.Id == 216177905712103424 || Context.Message.Author.Id == 466926694431719424)
            {
                id ??= Context.Guild.DefaultChannel.Id;

                await coreService.BackupServerAsync(id.Value, false);
            }
        }

        [Command("add-quote")]
        [Summary("Add a quote in the correct format to the default channel")]
        public async Task AddQuoteAsync(string author = null, [Remainder]string quote = null)
        {
            if (string.IsNullOrEmpty(author))
            {
                await Context.Channel.SendMessageAsync("Missing author");
                return;
            }

            if (string.IsNullOrEmpty(quote))
            {
                await Context.Channel.SendMessageAsync("Missing quote");
                return;
            }

            ulong id = Context.Guild.DefaultChannel.Id;
            quote = Regex.Unescape(quote);

            await Context.Guild.GetTextChannel(id).SendMessageAsync($"\"{quote}\" - {author} {DateTime.Now.Year}");

            string fileName = $"{Context.Guild.Name}-{Context.Guild.GetChannel(id).Name}-backup.csv";

            HashSet<CSVColumns> columns = new HashSet<CSVColumns>()
            {
                new CSVColumns
                {
                    MessageID = Context.Message.Id,
                    Author = author,
                    MessageContent = quote,
                    Timestamp = Context.Message.Timestamp
                }
            };

            fileService.WriteToCSV(columns, fileName);

            if (Context.Message.Channel.Id == id)
            {
                await Context.Message.DeleteAsync();
            }
        }

        [Command("random-quote")]
        [Summary("Gets a random quote from a given channel")]
        public async Task RandomQuoteAsync(ulong id = 453899130486521859)
        {
            Response<IMessage> response = await coreService.GetRandomQuoteAsync(id);
            IMessage randomQuote = response.Entity;

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithAuthor(randomQuote.Author)
                .WithColor(Color.Blue)
                .WithDescription(randomQuote.Content)
                .WithTimestamp(randomQuote.Timestamp);

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("backup-server")]
        [Summary("Backs up the current server to a CSV file")]
        public async Task BackupServerAsync()
        {
            await coreService.BackupServerAsync(Context.Guild.Id, true);
        }

        [Command("yotta-count")]
        [Summary("Gets a count of the items in the Yotta file")]
        public async Task YottaCountAsync()
        {
            HashSet<string> yotta = File.ReadAllLines($"{Context.Guild.Name}\\Yotta.txt").ToHashSet();
            List<PrependEnum> enumValues = Enum.GetValues(typeof(PrependEnum)).Cast<PrependEnum>().ToList();

            StringBuilder sb = new StringBuilder();

            foreach (PrependEnum item in enumValues)
            {
                int prependCount = yotta.Where(x => x == item.ToString()).Count();
                sb.AppendFormat("{0}: {1}\n", item, prependCount);
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("yotta-ordered")]
        [Summary("Reads the Yotta count and gives an ordered representation of it")]
        public async Task YottaOrderedAsync()
        {
            HashSet<string> yotta = File.ReadAllLines("Yotta.txt").OrderBy(x => x).ToHashSet();

            await Context.Channel.SendMessageAsync(string.Join(", ", yotta));
        }

        [Command("emoji-usage")]
        [Summary("Gets the usage of a given emoji in the guild")]
        public async Task EmojiUsageAsync(string emoteName)
        {
            if (Emote.TryParse($":{emoteName}:", out Emote emote))
            {
                await EmoteUsage(emote);
            }
        }

        [Command("emoji-usage")]
        [Summary("Gets the usage of a given emoji in the guild")]
        public async Task EmojiUsageAsync(ulong emojiId)
        {
            Response<GuildEmote> emote = await coreService.GetEmoteFromGuild(emojiId, Context.Guild);

            if (emote.HasData)
            {
                await EmoteUsage(emote.Entity);
            }
        }

        private async Task EmoteUsage(Emote emote)
        {
            if (emote != null)
            {
                ISocketMessageChannel channel = Context.Channel;

                ListResponse<IMessage> messagesResponse = await coreService.GetAllMessagesAsync(channel.Id);

                if (messagesResponse.HasData)
                {
                    IEnumerable<IMessage> messages = messagesResponse.Entities;

                    Regex emoteRegex = new Regex(@"\<(\:.*?(\b" + emote.Name.ToLowerInvariant() + @"\b)\:)(.*?\d)\>", RegexOptions.IgnoreCase);

                    HashSet<IMessage> filteredMessages = messages.Where(m => emoteRegex.IsMatch(m.Content)).ToHashSet();

                    string message = string.Format(":{0}: has been used {1} times in {2} messages ({3})", emote.Name, filteredMessages.Count, messages.Count(), channel.Name);
                    await channel.SendMessageAsync(message);
                }
            }
        }

        [Command("shutdown")]
        [Summary("Shuts down the bot")]
        public async Task ShutdownAsync()
        {
            await Context.Channel.SendMessageAsync("Shutting down...");

            await botService.DisconnectAsync();

            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.Kill();
        }

        [Command("change-boticon")]
        [Summary("Changes the icon of the bot")]
        public async Task ChangeIconAsync(string imageUri)
        {
            await coreService.ChangeIconAsync(imageUri);
        }

        [Command("ping-server")]
        [Summary("Pings a server with a given name or address")]
        public async Task PingServer(string ipAddress)
        {
            Response<IPStatus> status = await coreService.PingHostAsync(ipAddress);
            await Context.Channel.SendMessageAsync($"{ipAddress} responded with status {status.Entity}");
        }
    }
}
