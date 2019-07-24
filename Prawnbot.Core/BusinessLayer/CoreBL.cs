using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Log;
using Prawnbot.Core.Model.API.Giphy;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Core.Utility;
using Prawnbot.Utility.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface ICoreBL
    {
        Task SendImageFromBlobStore(string fileName);
        Task<bool> ContainsUser(SocketUserMessage message);
        Task<bool> ContainsText(SocketUserMessage message);
        string FlipACoin(string headsValue, string tailsValue);
        Task<string[]> YottaPrepend();
        /// <summary>
        /// Method to set the status of the bot
        /// </summary>
        /// <param name="status">UserStatus to change to</param>
        /// <returns>Task</returns>
        Task<bool> SetBotStatusAsync(UserStatus status);
        /// <summary>
        /// Updates the bot's rich presence
        /// </summary>
        /// <param name="name">Name of the game</param>
        /// <param name="activityType">Activity type of the game</param>
        /// <param name="streamUrl">(Optional) stream url</param>
        /// <returns></returns>
        Task UpdateRichPresenceAsync(string name, ActivityType activityType, string streamUrl);
        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        SocketGuildUser GetUser(string username);
        /// <summary>
        /// Get all the current users in the servers the bot is connected to
        /// </summary>
        /// <returns></returns>
        List<SocketGuildUser> GetAllUsers();
        /// <summary>
        /// Gets all messages from a guild text channel
        /// </summary>
        /// <param name="id">Text channel ID</param>
        /// <returns></returns>
        Task<IList<IMessage>> GetAllMessagesAsync(ulong id);
        /// <summary>
        /// Get a guild by name
        /// </summary>
        /// <param name="guildName">Name of the guild</param>
        /// <returns></returns>
        SocketGuild GetGuild(string guildName);
        /// <summary>
        /// Get all the current guilds the bot is connected to
        /// </summary>
        /// <returns>List of IGuilds</returns>
        List<SocketGuild> GetAllGuilds();
        /// <summary>
        /// Gets the default channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel FindDefaultChannel(SocketGuild guild);
        /// <summary>
        /// Gets a text channel with the supplied ID
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(ulong id);
        /// <summary>
        /// Gets a text channel with the supplied guild and id
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, ulong id);
        /// <summary>
        /// Gets a text channel with a supplied guild and name
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="channelName"></param>
        /// <returns></returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, string channelName);
        /// <summary>
        /// Gets a channel of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <param name="channel">Channel</param>
        /// <returns>SocketTextChannel</returns>
        SocketTextChannel FindTextChannel(SocketGuild guild, SocketTextChannel channel); 
        /// <summary>
        /// Gets all the channels of the given guild
        /// </summary>
        /// <param name="guild">Server</param>
        /// <returns>IReadOnlyCollection of SocketTextChannels</returns>
        List<SocketTextChannel> FindGuildTextChannels(SocketGuild guild);
        /// <summary>
        /// Create an FFMPEG process for the audio commands
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Process CreateFfmpegProcess(string path);
        /// <summary>
        /// Leaves the current audio channel
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        Task LeaveAudioAsync(IGuild guild);
        /// <summary>
        /// Sends a message to a channel
        /// </summary>
        /// <param name="guild">Guild to send it to</param>
        /// <param name="channel">Channel to send it to</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendChannelMessageAsync(SocketGuild guild, SocketTextChannel channel, string messageText);
        string FindEmojis(SocketUserMessage message);
        /// <summary>
        /// Send a message through a DM
        /// </summary>
        /// <param name="user">User to DM</param>
        /// <param name="messageText">Text of the message</param>
        /// <returns></returns>
        Task SendDMAsync(SocketGuildUser user, string messageText);
        Task ReactToMessageAsync(string Content, string[] lookupValues, string replyText, bool giffedMessage = false, string gifSearchText = null);
        string TagUser(ulong id);
        bool PingHost(string nameOrAddress);
        Task<IMessage> GetRandomQuoteAsync(ulong id);
        Task BackupServerAsync(ulong id, bool server);
        Task<GuildEmote> GetEmoteFromGuild(ulong id, SocketGuild guild);
    }

    public class CoreBL : BaseBL, ICoreBL
    {
        private readonly IFileBL fileBL;
        private readonly IAPIBL apiBL;
        private readonly ILogging logging;

        public CoreBL(IFileBL fileBL, IAPIBL apiBL, ILogging logging)
        {
            this.fileBL = fileBL;
            this.apiBL = apiBL;
            this.logging = logging;
        }

        private static string StrippedMessage { get; set; }
        protected readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task<bool> ContainsUser(SocketUserMessage message)
        {
            StrippedMessage = message.Content.RemoveSpecialCharacters();

            if (StrippedMessage.ContainsSingleLower("sam") || message.IsUserTagged(258627811844030465))
            {
                List<GiphyDatum> gifs = await apiBL.GetGifsAsync("calendar");
                await Context.Channel.SendMessageAsync($"Have you put it in the calendar? \n{gifs.RandomOrDefault().bitly_gif_url}");
            }
            if (StrippedMessage.ContainsSingleLower("ilja") || StrippedMessage.ContainsSingleLower("ultratwink") || message.IsUserTagged(341940376057282560)) await Context.Channel.SendMessageAsync("Has terminal gay");
            if (StrippedMessage.ContainsSingleLower("cam") || StrippedMessage.ContainsSingleLower("cameron") || message.IsUserTagged(216177905712103424)) await Context.Channel.SendMessageAsync("*Father Cammy");

            if (ConfigUtility.YottaMode && (StrippedMessage.ContainsSingleLower("sean") || message.IsUserTagged(201371614489608192) || StrippedMessage.ContainsSingleLower("seans")))
            {
                string[] yotta = await YottaPrepend();
                string yottaFull = string.Join(", ", yotta);

                await Context.Channel.SendMessageAsync($"{yottaFull} {(yottaFull.Length != 0 ? 'c' : 'C')}had Sean, stud of the co-op, leader of the Corfe Mullen massive");
            }

            return true;
        }

        public async Task<bool> ContainsText(SocketUserMessage message)
        {
            StrippedMessage = message.Content.RemoveSpecialCharacters();

            #region Config
            if (ConfigUtility.DadMode)
            {
                await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "kys" }, replyText: $"Alright {Context.User.Mention}, that was very rude. Instead, take your own advice.");
                await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "dad" }, replyText: "404 dad not found");
            }

            if (ConfigUtility.DadMode && StrippedMessage.StartsWith("im"))
            {
                List<string> messageArray = message.Content.ToLowerInvariant().Split(' ').ToList();

                if (messageArray[0].RemoveSpecialCharacters() == "im" && messageArray.Count() != 1)
                {
                    messageArray.RemoveAt(0);

                    await Context.Channel.SendMessageAsync($"Hi {string.Join(' ', messageArray.ToList())}, i'm dad");
                }
            }

            if (ConfigUtility.ProfanityFilter)
            {
                if (await apiBL.GetProfanityFilterAsync(StrippedMessage))
                {
                    var gifs = await apiBL.GetGifsAsync("swearing", 50);
                    await Context.Channel.SendMessageAsync(gifs.RandomOrDefault().bitly_gif_url);
                }
            }
            #endregion

            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "daddy" }, replyText: $"{Context.User.Mention} you can be my daddy if you want :wink:");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "africa" }, replyText: "toto by africa");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "big" }, replyText: "chunky");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "round" }, replyText: "plumpy");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "huge" }, replyText: "Girl, you huge");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "marvin" }, replyText: "Marvout");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "marvout" }, replyText: "Marvin");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "engineer" }, replyText: "The engineer is engihere");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "ban" }, replyText: "Did I hear somebody say Macro pad?");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "2realirl4meirl" }, replyText: "REEEEEEEEEEEEEEEEEEE");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "bruh" }, replyText: "https://www.youtube.com/watch?v=2ZIpFytCSVc");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "what", "can", "i", "say" }, replyText: "except, you're welcome!");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "oi", "oi" }, replyText: "big boi");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "plastic", "bag" }, replyText: "Drifting through the wind?");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "you", "so" }, replyText: $"you so\nfucking\nprecious\nwhen you\nsmile\nhit it\nfrom the \nback and\ndrive you\nwild");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "who", "can", "relate" }, replyText: "I don't know");
            await ReactToMessageAsync(message.Content, lookupValues: new string[] { "!skip" }, replyText: "you fucking what", giffedMessage: true, gifSearchText: "what");
            await ReactToMessageAsync(StrippedMessage, lookupValues: new string[] { "pipe", "down" }, replyText: "pipe down", giffedMessage: true, gifSearchText: "pipe down");

            if (StrippedMessage.ContainsSingleLower("wheel") || StrippedMessage.ContainsSingleLower("bus"))
            {
                await Context.Channel.SendMessageAsync(new List<string>() { "The wheels on the bus go round and round", "Excuse me sir, you can't have wheels in this area" }.RandomItemFromList());
            }

            if (StrippedMessage.ContainsSingleLower("hentai"))
            {
                var apiResult = await apiBL.Rule34PostsAsync(new string[] { "blonde" });
                var randomResult = apiResult.RandomOrDefault();

                WebRequest req = WebRequest.Create(randomResult.file_url);
                using Stream stream = req.GetResponse().GetResponseStream();
                await Context.Channel.SendFileAsync(stream, $"{randomResult.id}.png", isSpoiler: true);
            }

            //if (StrippedMessage.ContainsSingleLower("kowalski") || StrippedMessage.ContainsSingleLower("analysis"))  await SendGiffedMessage("kowalski", $"{TagUser(147860921488900097)} analysis");

            if (StrippedMessage.ContainsSingleLower("uwu") || StrippedMessage.ContainsManyLower(new string[] { "u", "w", "u" }))
            {
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync("This is an uwu free zone");
            }

            if (StrippedMessage.ContainsManyLower(new string[] { "top", "elims" }) || StrippedMessage.ContainsManyLower(new string[] { "gold", "elims" })) await SendImageFromBlobStore("top_elims.png");
            if (StrippedMessage.ContainsManyLower(new string[] { "taps", "head" })) await SendImageFromBlobStore("james_tapping_head.png");
            if (StrippedMessage.ContainsManyLower(new string[] { "one", "last", "ride" })) await SendImageFromBlobStore("one_last_ride.png");
            if (StrippedMessage.ContainsManyLower(new string[] { "cam", "murray" })) await SendImageFromBlobStore("cam_murray.png");

            return true;
        }

        public async Task SendImageFromBlobStore(string fileName)
        {
            await Context.Message.AddReactionAsync(new Emoji("👍"));

            Uri uri = await fileBL.GetUriFromBlobStore(fileName, "botimages");

            WebRequest req = WebRequest.Create(uri);
            using Stream stream = req.GetResponse().GetResponseStream();
            await Context.Channel.SendFileAsync(stream, fileName);
        }

        public async Task<bool> SetBotStatusAsync(UserStatus status)
        {
            try
            {
                await Client.SetStatusAsync(status);
                return true;
            }
            catch (Exception e)
            {
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "BotStatus", "Error setting bot status", e));
                return false;
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
                await logging.PopulateEventLog(new LogMessage(LogSeverity.Error, "RichPresence", "Error occured while updating rich presence", e));
            }
        }

        public SocketGuildUser GetUser(string username)
        {
            try
            {
                List<SocketGuildUser> users = GetAllUsers();
                return users.Where(x => x.Username == username).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SocketGuildUser> GetAllUsers()
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (SocketGuild guild in Client.Guilds)
            {
                foreach (SocketGuildUser user in guild.Users)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<IList<IMessage>> GetAllMessagesAsync(ulong id)
        {
            RequestOptions options = new RequestOptions
            {
                Timeout = 2000,
                RetryMode = RetryMode.RetryTimeouts,
                CancelToken = CancellationToken.None
            };

            IEnumerable<IMessage> messages = await FindTextChannel(id).GetMessagesAsync(limit: 500000, options: options).FlattenAsync();

            return messages.Reverse().ToList();
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

        public List<SocketGuild> GetAllGuilds()
        {
            return Client.Guilds.ToList();
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

        public List<SocketTextChannel> FindGuildTextChannels(SocketGuild guild)
        {
            return Client.Guilds.FirstOrDefault(x => x == guild).TextChannels.ToList();
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

            var regex = new Regex(@"(\<(\:.*?\:)(.*?\d)\>)", RegexOptions.IgnoreCase);
            var matches = regex.Matches(message.Content);

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

        public async Task ReactToMessageAsync(string Content, string[] lookupValues, string replyText, bool giffedMessage = false, string gifSearchText = null)
        {
            List<GiphyDatum> gifs = new List<GiphyDatum>();

            if (giffedMessage && gifSearchText != null) gifs = await apiBL.GetGifsAsync(gifSearchText);
            string replyMessage = giffedMessage && gifs != null 
                    ? $"{replyText}\n{gifs.RandomOrDefault().bitly_gif_url}" 
                    : replyText;

            if (lookupValues.Count() > 1)
            {
                if (Content.ContainsManyLower(lookupValues)) await Context.Channel.SendMessageAsync(replyMessage);
            }
            else
            {
                if (Content.ContainsSingleLower(lookupValues.First())) await Context.Channel.SendMessageAsync(replyMessage);
            }
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
            return new List<string>
            {
               headsValue ?? "HEADS",
               tailsValue ?? "TAILS"
            }.RandomItemFromList();
        }

        public async Task<string[]> YottaPrepend()
        {
            string[] fileContents = await fileBL.ReadFromFileAsync($"{Context.Guild.Name}\\Yotta.txt");

            Dictionary<string, int> valueDictionary = new Dictionary<string, int>();

            string[] enumValues = (string[])Enum.GetValues(typeof(PrependEnum));

            foreach (string enumValue in enumValues)
            {
                valueDictionary.Add(enumValue, 0);
            }

            foreach (string line in fileContents)
            {
                valueDictionary[line]++;
            }

            bool validValue = false;

            while (!validValue)
            {
                List<string> invalidValues = new List<string>();

                var randomEnumValue = enumValues.RandomItemFromList(); // enumValues.GetValue(random.Next(enumValues.Length)).ToString();

                if (valueDictionary[randomEnumValue] < 69)
                {
                    await fileBL.WriteToFile(randomEnumValue, $"{Context.Guild.Name}\\Yotta.txt");
                    fileContents.Append(randomEnumValue);
                    break;
                }
                else
                {
                    invalidValues.Add(randomEnumValue);

                    if (invalidValues.Count == enumValues.Length) break;
                }
            }

            return fileContents.Reverse().ToArray();
        }

        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public async Task<string> GetLanguageFullName(string origin)
        {
            var languages = await apiBL.GetLanguagesAsync();

            return languages.FirstOrDefault().Languages.SelectMany(x => x.LanguageDetails).Where(y => y.dir == origin).FirstOrDefault().name;
        }

        public async Task<IMessage> GetRandomQuoteAsync(ulong id)
        {
            IList<IMessage> quoteRoom = await GetAllMessagesAsync(id); 
            return quoteRoom.RandomItemFromList();
        }

        public async Task BackupServerAsync(ulong id, bool server)
        {
            await Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string fileName = server
                    ? $"{FindTextChannel(id).Name}-backup.csv"
                    : $"{Context.Guild.Name}-backup.csv";

                await Context.Channel.SendMessageAsync($"Started {(server ? "server" : "channel")} backup of {(server ? Context.Guild.Name : Context.Guild.GetTextChannel(id).Name)} {(server ? "(" + Context.Guild.TextChannels.Count() + " channels)" : "")} at {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

                if (server)
                {
                    foreach (SocketTextChannel textChannel in Context.Guild.TextChannels)
                    {
                        IList<IMessage> messagesToAdd = await GetAllMessagesAsync(textChannel.Id);
                        List<CSVColumns> records = fileBL.CreateCSVList(messagesToAdd);
                        fileBL.WriteToCSV(records, textChannel.Id, fileName);
                    }
                }
                else
                {
                    IList<IMessage> messagesToAdd = await GetAllMessagesAsync(id);

                    var records = fileBL.CreateCSVList(messagesToAdd);
                    fileBL.WriteToCSV(records, id, fileName);
                }

                stopwatch.Stop();

                await Context.Channel.SendMessageAsync($"Finished {(server ? "server" : "channel")} backup of {(server ? Context.Guild.Name : Context.Guild.GetTextChannel(id).Name)} to {fileName}. \nThe operation took {stopwatch.Elapsed.Hours}h:{stopwatch.Elapsed.Minutes}m:{stopwatch.Elapsed.Seconds}s:{stopwatch.Elapsed.Milliseconds}ms");
            });
        }

        public async Task<GuildEmote> GetEmoteFromGuild(ulong id, SocketGuild guild) 
        {
            GuildEmote emote = await guild.GetEmoteAsync(id);

            return emote;
        }
    }
}
