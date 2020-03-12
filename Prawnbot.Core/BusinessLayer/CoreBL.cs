using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Prawnbot.Common;
using Prawnbot.Common.Configuration;
using Prawnbot.Common.DTOs;
using Prawnbot.Common.DTOs.API.Giphy;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Custom.Attributes;
using Prawnbot.Core.Custom.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.FileHandling.Interfaces;
using Prawnbot.Infrastructure;
using Prawnbot.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        private readonly ILogging logging;
        private readonly IConfigUtility configUtility;        
        private readonly IAzureStorageService azureStorageService;

        private int EventsTriggered { get; set; }
        private bool MessageSent { get; set; }

        public CoreBL(IFileBL fileBL, IAPIBL apiBL, ILogging logging, IConfigUtility configUtility, IAzureStorageService azureStorageService)
        {
            this.fileBL = fileBL;
            this.apiBL = apiBL;
            this.logging = logging;
            this.configUtility = configUtility;            
            this.azureStorageService = azureStorageService;
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
                IBunch<string> yotta = await YottaPrependAsync();
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
                    IBunch<GiphyDatum> gifs = await apiBL.GetGifsAsync("swearing", 50);
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
                await Context.Channel.SendMessageAsync(new Bunch<string>() { "The wheels on the bus go round and round", "Excuse me sir, you can't have wheels in this area" }.RandomOrDefault());
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
                logging.Log_Info($"Message recieved from {Context.Message.Author.Username} ({Context.Guild.Name}): \"{Context.Message.Content}\"");
            }

            EventsTriggered = 0;
            MessageSent = false;
        }

        public async Task ReactToTaggedUserWithGifAsync(SocketUserMessage message, ulong userId, string replyMessage, string gifSearchText)
        {
            IBunch<GiphyDatum> gifs = gifSearchText != null
            ? await apiBL.GetGifsAsync(gifSearchText)
            : new Bunch<GiphyDatum>();

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
            IBunch<GiphyDatum> gifs = gifSearchText != null
                ? await apiBL.GetGifsAsync(gifSearchText)
                : new Bunch<GiphyDatum>();

            replyMessage += $"\n{gifs.RandomOrDefault().bitly_gif_url}";

            await ReactToSingleWordAsync(Content, lookupValue, replyMessage);
        }

        public async Task ReactToMultipleWordsWithGifAsync(string Content, string[] lookupValues, string replyMessage, string gifSearchText)
        {
            IBunch<GiphyDatum> gifs = gifSearchText != null
                ? await apiBL.GetGifsAsync(gifSearchText)
                : new Bunch<GiphyDatum>();

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
            await Context.Message.AddReactionAsync(new Emoji("👍"));

            Response<Uri> uri = await azureStorageService.GetUriFromBlobStoreAsync(fileName, "botimages");

            WebRequest req = WebRequest.Create(uri.Entity);
            using Stream stream = req.GetResponse().GetResponseStream();
            await Context.Channel.SendFileAsync(stream, fileName);
        }

        public async Task SetBotStatusAsync(UserStatus status = UserStatus.Online)
        {
            try
            {
                await Client.SetStatusAsync(status);
            }
            catch (Exception e)
            {
                logging.Log_Exception(e, optionalMessage: "Error setting bot status");
                return;
            }
        }

        public async Task UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl)
        {
            try
            {
                await Client.SetGameAsync(name, activityType == ActivityType.Streaming ? streamUrl : null, activityType);
            }
            catch (Exception e)
            {
                logging.Log_Exception(e, optionalMessage: "Error occured while updating rich presence");
            }
        }

        public SocketGuildUser GetUser(string username)
        {
            try
            {
                IBunch<SocketGuildUser> users = GetAllUsers();
                return users.Where(x => x.Username == username).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IBunch<SocketGuildUser> GetAllUsers()
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (SocketGuild guild in Client.Guilds)
            {
                foreach (SocketGuildUser user in guild.Users)
                {
                    users.Add(user);
                }
            }

            return users.ToBunch();
        }

        public async Task<IBunch<IMessage>> GetAllMessagesAsync(ulong id, int limit = 100000)
        {
            RequestOptions options = new RequestOptions
            {
                Timeout = 5000,
                RetryMode = RetryMode.RetryTimeouts,
                CancelToken = CancellationToken.None
            };

            SocketTextChannel channel = FindTextChannel(id);

            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(limit: limit, options: options).FlattenAsync();

            return messages.Reverse().ToBunch();
        }

        public async Task<IBunch<IMessage>> GetUserMessagesAsync(ulong id, int limit = 100000)
        {
            IBunch<IMessage> messages = await GetAllMessagesAsync(id, limit);

            return messages.Where(x => x.Type == MessageType.Default && !x.Author.IsBot && !x.Author.IsWebhook).ToBunch();
        }

        public async Task<IBunch<IMessage>> GetAllMessagesByTimestampAsync(ulong guildId, DateTime timestamp)
        {
            IList<IMessage> allMessages = await GetAllMessagesAsync(guildId);
            return allMessages.Where(x => x.Timestamp == timestamp || x.EditedTimestamp == timestamp).ToBunch();
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

        public IBunch<SocketGuild> GetAllGuilds()
        {
            return Client.Guilds.ToBunch();
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

        public IBunch<SocketTextChannel> FindGuildTextChannels(SocketGuild guild)
        {
            return Client.Guilds.FirstOrDefault(x => x == guild).TextChannels.ToBunch();
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

        public string FlipACoin(string headsValue, string tailsValue)
        {
            return new Bunch<string>
            {
               headsValue ?? "HEADS",
               tailsValue ?? "TAILS"
            }.Random();
        }

        public async Task<IBunch<string>> YottaPrependAsync()
        {
            Random random = new Random();

            IBunch<string> fileContents = await fileBL.ReadFromFileAsync($"{Context.Guild.Name}\\Yotta.txt");

            Dictionary<PrependEnum, int> valueDictionary = new Dictionary<PrependEnum, int>();

            PrependEnum[] enumValues = (PrependEnum[])Enum.GetValues(typeof(PrependEnum));

            foreach (PrependEnum enumValue in enumValues)
            {
                valueDictionary.Add(enumValue, 0);
            }

            foreach (string line in fileContents)
            {
                valueDictionary[(PrependEnum)Enum.Parse(typeof(PrependEnum), line)]++;
            }

            bool validValue = false;

            while (!validValue)
            {
                IBunch<PrependEnum> invalidValues = new Bunch<PrependEnum>();

                PrependEnum randomEnumValue = (PrependEnum)enumValues.GetValue(random.Next(enumValues.Length)); //.GetValue().ToString();

                if (valueDictionary[randomEnumValue] < 69)
                {
                    await fileBL.WriteToFileAsync(randomEnumValue.ToString(), $"{Context.Guild.Name}\\Yotta.txt");
                    fileContents.Append(randomEnumValue.ToString());
                    break;
                }
                else
                {
                    invalidValues.Add(randomEnumValue);

                    if (invalidValues.Count == enumValues.Length)
                    {
                        break;
                    }
                }
            }

            fileContents.Reverse();
            return fileContents;
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
                    logging.Log_Exception(e, optionalMessage: $"Error occured pinging machine with name or address: {nameOrAddress}");

                    // Discard PingExceptions and return false;
                    return IPStatus.Unknown;
                }
            }
        }

        public async Task<string> GetLanguageFullName(string origin)
        {
            IBunch<LanguageTranslationRoot> languages = await apiBL.GetLanguagesAsync();

            return languages.FirstOrDefault().Languages.SelectMany(x => x.LanguageDetails).Where(y => y.dir == origin).FirstOrDefault().name;
        }

        public async Task<IMessage> GetRandomQuoteAsync(ulong id)
        {
            IBunch<IMessage> quoteRoom = await GetAllMessagesAsync(id);
            return quoteRoom.RandomOrDefault();
        }

        public async Task BackupServerAsync(ulong id, bool server)
        {
            await Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string fileName = server
                    ? $"{Context.Guild.Name}-backup.csv"
                    : $"{FindTextChannel(id).Name}-backup.csv";

                await Context.Channel.SendMessageAsync($"Started {(server ? "server" : "channel")} backup of {(server ? Context.Guild.Name : Context.Guild.GetTextChannel(id).Name)}{(server ? " (" + Context.Guild.TextChannels.Count() + " channels)" : "")} at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

                IBunch<CSVColumns> records = new Bunch<CSVColumns>();

                if (server)
                {
                    foreach (SocketTextChannel textChannel in Context.Guild.TextChannels)
                    {
                        IBunch<IMessage> messagesToAdd = await GetAllMessagesAsync(textChannel.Id);

                        IBunch<CSVColumns> channelMessages = fileBL.CreateCSVList(messagesToAdd);
                        records.AddRange(channelMessages);
                    }
                }
                else
                {
                    IBunch<IMessage> messagesToAdd = await GetAllMessagesAsync(id);

                    records = fileBL.CreateCSVList(messagesToAdd);
                }

                FileStream fileStream = fileBL.WriteToCSV(records, fileName);

                stopwatch.Stop();

                await Context.Channel.SendFileAsync(fileStream, fileName, $"Finished {(server ? "server" : "channel")} backup of {(server ? Context.Guild.Name : Context.Guild.GetTextChannel(id).Name)}. \nThe operation took {stopwatch.Elapsed.Hours}h:{stopwatch.Elapsed.Minutes}m:{stopwatch.Elapsed.Seconds}s:{stopwatch.Elapsed.Milliseconds}ms");

                fileStream.Close();
                fileStream.Dispose();
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

            builder.WithTitle(botInfo.Name)
                .WithColor(Color.Blue)
                .WithDescription(
                $"Bot owner: {botInfo.Owner.Username} \n" +
                $"Created at: {botInfo.CreatedAt}\n" +
                $"Description: {botInfo.Description}"
                );

            await Context.Channel.SendMessageAsync("", UseTTS, builder.Build());
        }

        public async Task CommandsAsync(bool includeNotImplemented)
        {
            EmbedBuilder builder = new EmbedBuilder();

            List<MethodInfo> methods = typeof(Modules.Modules).Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => (m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0) && (!m.CustomAttributes.Any(y => y.AttributeType == typeof(NotImplementedAttribute)) && !includeNotImplemented))
                      .OrderBy(x => x.Name)
                      .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (MethodInfo method in methods)
            {
                IEnumerable<CustomAttributeData> discordAttributes = method.CustomAttributes.Where(x => x.AttributeType == typeof(SummaryAttribute) || x.AttributeType == typeof(CommandAttribute));
                string commandAttribute = discordAttributes.First(x => x.AttributeType == typeof(CommandAttribute)).ConstructorArguments.First().Value.ToString();
                string summaryAttribute = discordAttributes.FirstOrDefault(x => x.AttributeType == typeof(SummaryAttribute))?.ConstructorArguments.First().Value.ToString() ?? "No summary available";

                sb.AppendLine($"{configUtility.CommandDelimiter}{commandAttribute}: {summaryAttribute}");
            }

            builder.WithTitle($"Commands | All commands follow the structure {configUtility.CommandDelimiter}(command)")
                .WithColor(Color.Blue)
                .WithDescription(sb.ToString());

            if (Context.Channel.GetType() != typeof(SocketDMChannel))
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention}: pm'd with command details!");
            }
                
            await Context.User.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        public async Task StatusAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle($"Status: {Context.Guild.Name}")
                .WithColor(Color.Blue)
                .WithDescription(
                $"The default channel is: \"{Context.Guild.DefaultChannel}\" \n" +
                $"The server was created on {Context.Guild.CreatedAt.LocalDateTime.ToString("dd/MM/yyyy")} \n" +
                $"The server currently has {Context.Guild.Users.Where(x => !x.IsBot).Count()} users and {Context.Guild.Users.Where(x => x.IsBot).Count()} bots ({Context.Guild.MemberCount} total) \n" +
                $"The current AFK Channel is \"{Context.Guild.AFKChannel.Name}\"\n " +
                $"There are currently {Context.Guild.TextChannels.Count} text channels and {Context.Guild.VoiceChannels.Count} voice channels in the server\n " +
                $"The server owner is {Context.Guild.Owner}")
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync(string.Empty, UseTTS, builder.Build());
        }

        public async Task ChangeNicknameAsync(string guildName, string nickname)
        {
            SocketGuild guild = GetGuild(guildName);

            await guild.CurrentUser.ModifyAsync(x => x.Nickname = nickname);
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
