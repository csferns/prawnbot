using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Attributes;
using Prawnbot.Core.BlobStorage;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public class CoreBL : BaseBL, ICoreBL
    {
        private readonly IFileBL fileBL;
        private readonly IAPIBL apiBL;
        private readonly ILogger<CoreBL> logger;
        private readonly IConfigUtility configUtility;
        private readonly IBlobStorage blobStorage;

        private int EventsTriggered { get; set; }
        private bool MessageSent { get; set; }

        public CoreBL(IFileBL fileBL, IAPIBL apiBL, ILogger<CoreBL> logger, IConfigUtility configUtility, IBlobStorage blobStorage)
        {
            this.fileBL = fileBL;
            this.apiBL = apiBL;
            this.logger = logger;
            this.configUtility = configUtility;
            this.blobStorage = blobStorage;
        }

        private string StrippedMessage { get; set; }
        protected readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task MessageEventListeners(SocketUserMessage message)
        {
            if ((message.ContainsEmote() || message.ContainsEmoji()) && configUtility.EmojiRepeat)
            {
                await Context.Channel.SendMessageAsync(FindEmojis(message));
                EventsTriggered++;
            }

            StrippedMessage = message.Content.RemoveSpecialCharacters();

            await ReactToSingleWordWithGifAsync(StrippedMessage, lookupValue: "sam", replyMessage: "Have you put it in the calendar?", gifSearchText: "calendar");
            await ReactToTaggedUserWithGifAsync(message, userId: 258627811844030465, replyMessage: "Have you put it in the calendar?", gifSearchText: "calendar");

            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "ilja", replyMessage: "Has terminal gay");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "ultratwink", replyMessage: "Has terminal gay");
            await ReactToTaggedUserAsync(message, userId: 341940376057282560, replyMessage: "Has terminal gay");

            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "cam", replyMessage: "*Father Cammy");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "cameron", replyMessage: "*Father Cammy");
            await ReactToTaggedUserAsync(message, userId: 216177905712103424, replyMessage: "*Father Cammy");

            if (configUtility.YottaMode && (StrippedMessage.ContainsSingleLower("sean") || message.IsUserTagged(201371614489608192) || StrippedMessage.ContainsSingleLower("seans")))
            {
                HashSet<string> yotta = await YottaPrependAsync();
                string yottaFull = string.Join(", ", yotta);

                await Context.Channel.SendMessageAsync($"{yottaFull} {(yottaFull.Length > 0 ? 'c' : 'C')}had Sean, stud of the co-op, leader of the Corfe Mullen massive");
                EventsTriggered++;
            }

            #region Config
            if (configUtility.DadMode)
            {
                await ReactToSingleWordAsync(StrippedMessage, lookupValue: "kys", replyMessage: $"Alright {Context.User.Mention}, that was very rude. Instead, take your own advice.");
                await ReactToSingleWordAsync(StrippedMessage, lookupValue: "dad", replyMessage: "404 dad not found");
            }

            if (configUtility.DadMode && StrippedMessage.Contains("im"))
            {
                List<string> splitMessage = message.Content.ToLowerInvariant().Split(' ').ToList();

                string imString = splitMessage.Find(x => x == "im" || x == "i'm");
                int pos = splitMessage.IndexOf(imString);

                for (int i = 0; i < pos; i++)
                {
                    splitMessage.RemoveAt(i);
                }

                await Context.Channel.SendMessageAsync($"hello {string.Join(' ', splitMessage)}, i'm dad!");
            }

            if (configUtility.ProfanityFilter)
            {
                if (await apiBL.GetProfanityFilterAsync(StrippedMessage))
                {
                    HashSet<GiphyDatum> gifs = await apiBL.GetGifsAsync("swearing", 50);
                    await Context.Channel.SendMessageAsync(gifs.RandomOrDefault().bitly_gif_url);
                    EventsTriggered++;
                }
            }
            #endregion

            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "daddy", replyMessage: $"{Context.User.Mention} you can be my daddy if you want :wink:");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "africa", replyMessage: "toto by africa");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "big", replyMessage: "chunky");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "round", replyMessage: "plumpy");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "huge", replyMessage: "Girl, you huge");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "marvin", replyMessage: "Marvout");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "marvout", replyMessage: "Marvin");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "engineer", replyMessage: "The engineer is engihere");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "ban", replyMessage: "Did I hear somebody say Macro pad?");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "2realirl4meirl", replyMessage: "REEEEEEEEEEEEEEEEEEE");
            await ReactToSingleWordAsync(StrippedMessage, lookupValue: "bruh", replyMessage: "https://www.youtube.com/watch?v=2ZIpFytCSVc");

            await ReactToSingleWordWithGifAsync(StrippedMessage, lookupValue: "!skip", replyMessage: "you fucking what", gifSearchText: "what");

            await ReactToMultipleWordsAsync(StrippedMessage, lookupValues: new string[] { "what", "can", "i", "say" }, replyMessage: "except, you're welcome!");
            await ReactToMultipleWordsAsync(StrippedMessage, lookupValues: new string[] { "oi", "oi" }, replyMessage: "big boi");
            await ReactToMultipleWordsAsync(StrippedMessage, lookupValues: new string[] { "plastic", "bag" }, replyMessage: "Drifting through the wind?");
            await ReactToMultipleWordsAsync(StrippedMessage, lookupValues: new string[] { "you", "so" }, replyMessage: "you so\nfucking\nprecious\nwhen you\nsmile\nhit it\nfrom the \nback and\ndrive you\nwild");
            await ReactToMultipleWordsAsync(StrippedMessage, lookupValues: new string[] { "who", "can", "relate" }, replyMessage: "I don't know");

            await ReactToMultipleWordsWithGifAsync(StrippedMessage, lookupValues: new string[] { "pipe", "down" }, replyMessage: "", gifSearchText: "pipe down");

            if (StrippedMessage.ContainsSingleLower("wheel") || StrippedMessage.ContainsSingleLower("bus"))
            {
                await Context.Channel.SendMessageAsync(new HashSet<string>() { "The wheels on the bus go round and round", "Excuse me sir, you can't have wheels in this area" }.RandomOrDefault());
                EventsTriggered++;
            }

            await ReactToSingleWordWithGifAsync(StrippedMessage, lookupValue: "kowalski", replyMessage: $"{TagUser(147860921488900097)} analysis", gifSearchText: "kowalski");
            await ReactToSingleWordWithGifAsync(StrippedMessage, lookupValue: "analysis", replyMessage: $"{TagUser(147860921488900097)} analysis", gifSearchText: "kowalski");

            if (StrippedMessage.ContainsSingleLower("uwu"))
            {
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync("This is an uwu free zone");
                EventsTriggered++;
            }

            if (StrippedMessage.ContainsManyLower(new string[] { "top", "elims" }) || StrippedMessage.ContainsManyLower(new string[] { "gold", "elims" })) { await SendImageFromBlobStoreAsync("top_elims.png"); EventsTriggered++; }
            if (StrippedMessage.ContainsManyLower(new string[] { "taps", "head" })) { await SendImageFromBlobStoreAsync("james_tapping_head.png"); EventsTriggered++; }
            if (StrippedMessage.ContainsManyLower(new string[] { "one", "last", "ride" })) { await SendImageFromBlobStoreAsync("one_last_ride.png"); EventsTriggered++; }
            if (StrippedMessage.ContainsManyLower(new string[] { "cam", "murray" })) { await SendImageFromBlobStoreAsync("cam_murray.png"); EventsTriggered++; }

            if (EventsTriggered > 0)
            {
                logger.LogInformation("Message recieved from {0} ({1}): \"{2}\"", Context.Message.Author.Username, Context.Guild.Name, Context.Message.Content);
            }

            EventsTriggered = 0;
            MessageSent = false;
        }

        public async Task ReactToTaggedUserWithGifAsync(SocketUserMessage message, ulong userId, string replyMessage, string gifSearchText)
        {
            HashSet<GiphyDatum> gifs = gifSearchText != null
            ? await apiBL.GetGifsAsync(gifSearchText)
            : new HashSet<GiphyDatum>();

            replyMessage += $"\n{gifs.RandomOrDefault().bitly_gif_url}";

            await ReactToTaggedUserAsync(message, userId, replyMessage);
        }

        public async Task ReactToTaggedUserAsync(SocketUserMessage message, ulong userId, string replyMessage)
        {
            if (message.IsUserTagged(userId) && message.Content != null)
            {
                if (!MessageSent)
                {
                    await Context.Channel.SendMessageAsync(replyMessage);
                    MessageSent = true;
                }

                EventsTriggered++;
            }
        }

        public async Task ReactToSingleWordWithGifAsync(string Content, string lookupValue, string replyMessage, string gifSearchText)
        {
            HashSet<GiphyDatum> gifs = gifSearchText != null
                ? await apiBL.GetGifsAsync(gifSearchText)
                : new HashSet<GiphyDatum>();

            replyMessage += $"\n{gifs.RandomOrDefault().bitly_gif_url}";

            await ReactToSingleWordAsync(Content, lookupValue, replyMessage);
        }

        public async Task ReactToMultipleWordsWithGifAsync(string Content, string[] lookupValues, string replyMessage, string gifSearchText)
        {
            HashSet<GiphyDatum> gifs = gifSearchText != null
                ? await apiBL.GetGifsAsync(gifSearchText)
                : new HashSet<GiphyDatum>();

            replyMessage += $"\n{gifs.RandomOrDefault().bitly_gif_url}";

            await ReactToMultipleWordsAsync(Content, lookupValues, replyMessage);
        }

        public async Task ReactToSingleWordAsync(string Content, string lookupValue, string replyMessage)
        {
            if (Content.ContainsSingleLower(lookupValue))
            {
                if (!MessageSent)
                {
                    await Context.Channel.SendMessageAsync(replyMessage);
                    MessageSent = true;
                }

                EventsTriggered++;
            }
        }

        public async Task ReactToMultipleWordsAsync(string Content, string[] lookupValues, string replyMessage)
        {
            if (Content.ContainsManyLower(lookupValues))
            {
                if (!MessageSent)
                {
                    await Context.Channel.SendMessageAsync(replyMessage);
                    MessageSent = true;
                }

                EventsTriggered++;
            }
        }

        public async Task SendImageFromBlobStoreAsync(string fileName)
        {
            if (await blobStorage.ExistsAsync(BlobContainerEnum.BotImages, fileName))
            {
                await Context.Message.AddReactionAsync(new Emoji("👍"));

                using (MemoryStream stream = await blobStorage.DownloadAsync(BlobContainerEnum.BotImages, fileName))
                {
                    await Context.Channel.SendFileAsync(stream, fileName);
                }
            }
        }

        public async Task SetBotStatusAsync(UserStatus status = UserStatus.Online)
        {
            try
            {
                await Client.SetStatusAsync(status);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error setting bot status: {0}", e.Message);
            }
        }

        public async Task UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl)
        {
            try
            {
                await Client.SetGameAsync(name ?? null, activityType == ActivityType.Streaming ? streamUrl : null, activityType);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured while updating rich presence: {0}", e.Message);
            }
        }

        public SocketGuildUser GetUser(string username)
        {
            try
            {
                HashSet<SocketGuildUser> users = GetAllUsers();
                return users.Where(x => x.Username == username).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public HashSet<SocketGuildUser> GetAllUsers()
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (SocketGuild guild in Client.Guilds)
            {
                foreach (SocketGuildUser user in guild.Users)
                {
                    users.Add(user);
                }
            }

            return users.ToHashSet();
        }

        public async Task<HashSet<IMessage>> GetAllMessagesAsync(ulong id, int limit = 100000)
        {
            RequestOptions options = new RequestOptions
            {
                Timeout = 5000,
                RetryMode = RetryMode.RetryTimeouts,
                CancelToken = CancellationToken.None
            };

            SocketTextChannel channel = FindTextChannel(id);

            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(limit: limit, options: options).FlattenAsync();

            return messages.Reverse().ToHashSet();
        }

        public async Task<HashSet<IMessage>> GetUserMessagesAsync(ulong id, int limit = 100000)
        {
            HashSet<IMessage> messages = await GetAllMessagesAsync(id, limit);

            return messages.Where(x => x.Type == MessageType.Default && !x.Author.IsBot && !x.Author.IsWebhook).ToHashSet();
        }

        public async Task<HashSet<IMessage>> GetAllMessagesByTimestampAsync(ulong guildId, DateTime timestamp)
        {
            HashSet<IMessage> allMessages = await GetAllMessagesAsync(guildId);
            return allMessages.Where(x => x.Timestamp == timestamp || x.EditedTimestamp == timestamp).ToHashSet();
        }

        public SocketGuild GetGuild(string guildName)
        {
            try
            {
                return Client.Guilds.Where(x => x.Name == guildName).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public SocketGuild GetGuildById(ulong guildId)
        {
            return Client.GetGuild(guildId);
        }

        public HashSet<SocketGuild> GetAllGuilds()
        {
            return Client.Guilds.ToHashSet();
        }

        public SocketTextChannel FindDefaultChannel(SocketGuild guild)
        {
            return Client.Guilds.FirstOrDefault(x => x == guild).DefaultChannel;
        }

        public SocketTextChannel FindTextChannel(ulong id)
        {
            return Client.Guilds.SelectMany(x => x.TextChannels).FirstOrDefault(y => y.Id == id);
        }

        public SocketTextChannel FindTextChannel(SocketGuild guild, ulong id)
        {
            return guild.GetTextChannel(id);
        }

        public SocketTextChannel FindTextChannel(SocketGuild guild, string channelName)
        {
            return guild.TextChannels.Where(x => x.Name == channelName).FirstOrDefault();
        }

        public HashSet<SocketTextChannel> FindGuildTextChannels(SocketGuild guild)
        {
            return Client.Guilds.FirstOrDefault(x => x == guild).TextChannels.ToHashSet();
        }

        public SocketTextChannel FindTextChannel(SocketGuild guild, SocketTextChannel channel)
        {
            return Client.Guilds.FirstOrDefault(x => x == guild).TextChannels.FirstOrDefault(x => x == channel);
        }

        public Process CreateFfmpegProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public async Task LeaveAudioAsync(IGuild guild)
        {
            if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
            }
        }

        public async Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText)
        {
            await FindTextChannel(guild, channel).SendMessageAsync(messageText);
        }

        public async Task SendDMAsync(SocketGuildUser user, string messageText)
        {
            await UserExtensions.SendMessageAsync(user, messageText);
        }

        public string FindEmojis(SocketUserMessage message)
        {
            StringBuilder sb = new StringBuilder();

            Regex regex = new Regex(@"(\<(\:.*?\:)(.*?\d)\>)", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(message.Content);

            foreach (Match match in matches)
            {
                sb.Append(GetEmojiFromGuild(ulong.Parse(match.Groups[3].Value))).ToString();
            }

            return sb.ToString();
        }

        public GuildEmote GetEmojiFromGuild(ulong emojiId)
        {
            return Client.Guilds.SelectMany(x => x.Emotes).FirstOrDefault(x => x.Id == emojiId);
        }

        public string TagUser(ulong id)
        {
            StringBuilder sb = new StringBuilder("<@!");
            sb.Append(id);
            sb.Append('>');

            return sb.ToString();
        }

        public async Task<HashSet<string>> YottaPrependAsync()
        {
            Random random = new Random();
            string fileName = string.Format("{0}\\Yotta.json", Context.Guild.Name);

            using (FileStream fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                string contents = await reader.ReadToEndAsync();
                Dictionary<PrependEnum, int> fileContents = JsonConvert.DeserializeObject<Dictionary<PrependEnum, int>>(contents);

                List<PrependEnum> enumValues = Enum.GetValues(typeof(PrependEnum)).Cast<PrependEnum>().ToList();

                if (!fileContents.Any())
                {
                    foreach (PrependEnum item in enumValues)
                    {
                        fileContents.Add(item, 0);
                    }
                }

                PrependEnum enumValue = enumValues[random.Next(0, enumValues.Count() - 1)];

                if (!fileContents.ContainsKey(enumValue))
                {
                    fileContents.Add(enumValue, 0);
                }

                fileContents[enumValue]++;

                contents = JsonConvert.SerializeObject(fileContents);
                await writer.WriteAsync(contents);

                return fileContents.Select(x => x.Key.ToString())
                                   .OrderBy(x => x)
                                   .ToHashSet();
            }
        }

        public async Task<IPStatus> PingHostAsync(string nameOrAddress)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    int timeout = 5000;

                    IPAddress.TryParse(nameOrAddress, out IPAddress address);
                    Uri.TryCreate(nameOrAddress, UriKind.RelativeOrAbsolute, out Uri uri);

                    PingReply reply = null;

                    if (address != null)
                    {
                        reply = await pinger.SendPingAsync(address, timeout);
                    }
                    else if (uri != null)
                    {
                        reply = await pinger.SendPingAsync(uri.Host, timeout);
                    }

                    return reply?.Status ?? IPStatus.Unknown;
                    
                }
                catch (PingException e)
                {
                    logger.LogError(e, "Error occured pinging machine with name or address \"{0}\": {1}", nameOrAddress, e.Message);

                    // Discard PingExceptions and return false;
                    return IPStatus.Unknown;
                }
            }
        }

        public async Task<string> GetLanguageFullName(string origin)
        {
            HashSet<LanguageTranslationRoot> languages = await apiBL.GetLanguagesAsync();

            return languages.FirstOrDefault()
                            .Languages
                            .SelectMany(x => x.LanguageDetails)
                            .Where(y => y.dir == origin)
                            .FirstOrDefault()?.name;
        }

        public async Task<IMessage> GetRandomQuoteAsync(ulong id)
        {
            HashSet<IMessage> quoteRoom = await GetAllMessagesAsync(id);
            return quoteRoom.RandomOrDefault();
        }

        public async Task BackupServerAsync(ulong id, bool server)
        {
            await Task.Run(async () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                SocketGuild guild = Context.Guild;

                string startMessage = string.Format("Started server backup of {0} ({1} channels) at {2:dd/MM/yyyy HH:mm:ss}", guild.Name, guild.TextChannels.Count(), DateTime.Now);

                await Context.Channel.SendMessageAsync(startMessage);

                ConcurrentBag<CSVColumns> records = new ConcurrentBag<CSVColumns>();

                ParallelOptions options = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 3
                };

                Parallel.ForEach(guild.TextChannels, options, async (textChannel) =>
                {
                    HashSet<IMessage> messagesToAdd = await GetAllMessagesAsync(textChannel.Id);
                    HashSet<CSVColumns> channelMessages = fileBL.CreateCSVList(messagesToAdd);
                   // records.AddRange(channelMessages);
                });

                stopwatch.Stop();

                if (records.Any())
                {
                    string fileName = string.Format("{0}-backup.csv", guild.Name);

                    using (FileStream fileStream = fileBL.WriteToCSV(records.ToHashSet(), fileName))
                    {
                        string endMessage = string.Format("Finished server backup of {0} in {1:c}", guild.Name, stopwatch.Elapsed);

                        await Context.Channel.SendFileAsync(fileStream, fileName, endMessage);
                    }
                }
            });
        }

        public async Task BackupChannelAsync(ulong id)
        {
            await Task.Run(async () =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                SocketTextChannel channel = FindTextChannel(id);

                string startMessage = string.Format("Started channel backup of {0} at {1:dd/MM/yyyy HH:mm:ss}", channel.Name, DateTime.Now);

                await Context.Channel.SendMessageAsync(startMessage);

                HashSet<IMessage> messagesToAdd = await GetAllMessagesAsync(id);
                HashSet<CSVColumns> records = fileBL.CreateCSVList(messagesToAdd);

                stopwatch.Stop();

                if (records.Any())
                {
                    string fileName = string.Format("{0}-backup.csv", channel.Name);

                    using (FileStream fileStream = fileBL.WriteToCSV(records, fileName))
                    {
                        string endMessage = string.Format("Finished channel backup of {0} ({1} messages) in {2:c}", channel.Name, records.Count, stopwatch.Elapsed);
                        await Context.Channel.SendFileAsync(fileStream, fileName, endMessage);
                    }
                }
            });
        }

        public async Task<GuildEmote> GetEmoteFromGuildAsync(ulong id, SocketGuild guild)
        {
            GuildEmote emote = await guild.GetEmoteAsync(id);

            return emote;
        }

        public async Task GetBotInfoAsync() 
        { 
            RestApplication botInfo = await Context.Client.GetApplicationInfoAsync();

            EmbedBuilder builder = new EmbedBuilder();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Bot owner: {0}\n", botInfo.Owner.Username);
            sb.AppendFormat("Created at: {0:dd/MM/yyyy}\n", botInfo.CreatedAt);
            sb.AppendFormat("Description: {0}", botInfo.Description);

            builder.WithTitle(botInfo.Name)
                .WithColor(Color.Blue)
                .WithDescription(sb.ToString());

            await Context.Channel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        public async Task CommandsAsync(bool includeNotImplemented)
        {


            List<MethodInfo> methods = typeof(Modules.Modules).Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes<CommandAttribute>(inherit: false).Any() 
                               && (!m.GetCustomAttributes<NotImplementedAttribute>(inherit: false).Any() || includeNotImplemented))
                      .OrderBy(x => x.Name)
                      .ToList();


            HashSet<string> lines = new HashSet<string>();

            string delimiter = configUtility.CommandDelimiter;

            foreach (MethodInfo method in methods)
            {
                string command = method.GetCustomAttribute<CommandAttribute>(inherit: false).Text;
                string summary = method.GetCustomAttribute<SummaryAttribute>(inherit: false)?.Text ?? "No summary available";
                bool implemented = method.GetCustomAttribute<NotImplementedAttribute>(inherit: false) == null;

                List<System.Reflection.ParameterInfo> methodParams = method.GetParameters().ToList();
                string parameters = string.Join("\n\t", methodParams.Select(x => string.Format("[{0}]: {1}{2} {3}", methodParams.IndexOf(x) + 1, 
                                                                                                                implemented ? string.Empty : "(Not Implemented) ", 
                                                                                                                x.Name,
                                                                                                                x.IsOptional ? $"(default {x.DefaultValue ?? "NULL"})" : string.Empty)));

                if (!string.IsNullOrEmpty(parameters))
                {
                    parameters = "\n\t" + parameters;
                }

                lines.Add(string.Format("**{0}{1}**: {2}{3}\n", delimiter, command, summary, parameters));
            }

            await Context.User.SendMessageAsync($"Commands | All commands follow the structure {delimiter}(command)");

            StringBuilder sb = new StringBuilder();

            int messageLength = lines.Select(x => x.Length).Sum();

            if (messageLength > 2000)
            {
                foreach (string line in lines)
                {
                    if ((sb.Length + line.Length) > 2000)
                    {
                        await Context.User.SendMessageAsync(sb.ToString());
                        sb = new StringBuilder();
                    }

                    sb.Append(line);
                }

                if (sb.Length > 0)
                {
                    await Context.User.SendMessageAsync(sb.ToString());
                }
            }
            else if (messageLength < 2000 && messageLength > 0)
            {
                foreach (string line in lines)
                {
                    sb.Append(line);
                }

                await Context.User.SendMessageAsync(sb.ToString());
            }

            if (Context.Channel is not SocketDMChannel)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}: pm'd with command details!");
            }
        }

        public async Task StatusAsync()
        {
            SocketGuild guild = Context.Guild;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("The default channel is: {0}\n", guild.DefaultChannel);
            sb.AppendFormat("The server was created on {0:dd/MM/yyyy}\n", guild.CreatedAt);
            sb.AppendFormat("The server currently has {0} members and {1} bots ({2} total)\n", guild.Users.Where(x => !x.IsBot).Count(), guild.Users.Where(x => x.IsBot).Count(), guild.MemberCount);
            sb.AppendFormat("The current AFK channel is {0}\n", guild.AFKChannel.Name);
            sb.AppendFormat("There are currently {0} text channels and {1} voice channels\n", guild.TextChannels.Count, guild.VoiceChannels.Count);
            sb.AppendFormat("The server owner is {0}", guild.Owner.Nickname);

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Status: {guild.Name}")
                .WithColor(Color.Blue)
                .WithDescription(sb.ToString())
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        public async Task ChangeNicknameAsync(string guildName, string nickname)
        {
            SocketGuild guild = GetGuild(guildName);

            await guild.CurrentUser.ModifyAsync(x => x.Nickname = nickname);
        }

        public async Task ChangeIconAsync(string imageUri)
        {
            await ChangeIconAsync(new Uri(imageUri));
        }

        public async Task ChangeIconAsync(Uri imageUri = null)
        {
            if (imageUri != null)
            {
                WebRequest request = WebRequest.Create(imageUri);

                using (Stream stream = request.GetResponse().GetResponseStream())
                {
                    Image image = new Image(stream);
                    await Client.CurrentUser.ModifyAsync(x => x.Avatar = image);
                }
            }
            else
            {
                await Client.CurrentUser.ModifyAsync(x => x.Avatar = null);
            }
        }
    }
}
