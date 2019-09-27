using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Attributes;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Exceptions;
using Prawnbot.Core.Model.DTOs;
using Prawnbot.Core.ServiceLayer;
using Prawnbot.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ISpeechRecognitionService speechRecognitionService;
        public Modules(IBotService botService, ICoreService coreService, IFileService fileService, IAPIService apiService, ISpeechRecognitionService speechRecognitionService)
        {
            this.botService = botService;
            this.coreService = coreService;
            this.fileService = fileService;
            this.apiService = apiService;
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
        public async Task OrderMessageAsync()
        {
            SocketMessage message = Context.Channel.CachedMessages.OrderBy(x => x.Id).Last();
            string orderedMessage = message.Content.OrderBy(x => x).ToString();

            await Context.Channel.SendMessageAsync(orderedMessage);
        }


        [Command("random-user")]
        [Summary("Gets a random user from the guild and posts them")]
        public async Task RandomUserAsync()
        {
            SocketGuildUser randomUser = coreService.GetAllUsers().Entities.ToBunch().RandomOrDefault();

            await Context.Channel.SendMessageAsync(randomUser.Nickname ?? randomUser.Username);
        }

        [Command("flip-a-coin")]
        [Summary("Flips a coin")]
        public async Task FlipACoinAsync(string headsValue = null, string tailsValue = null)
        {
            string response = coreService.FlipACoin(headsValue, tailsValue).Entity;

            await Context.Channel.SendMessageAsync("Flipping coin...");
            await Task.Delay(1000);
            await Context.Channel.SendMessageAsync(response);
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
            if (copypastaName != "")
            {
                // TODO: Put this in the database or a text file
                string copypasta = copypastaName.ToLower() switch
                {
                    "owo" => "Rawr x3 nuzzlez how tha fuck is yo dirty ass pounces on you you so warm o3o notices you gotz a funky-ass bulge o: one of mah thugss aiiight :wink: nuzzlez yo' necky wecky~ murr~ hehehe rubbies yo' bulgy wolgy you so big-ass :oooo rubbies mo' on yo' bulgy wolgy it don't stop growin .///. kisses you n' lickies yo' necky daddy likies (; nuzzlez wuzzlez I hope daddy straight-up likes $: wigglez booty n' squirms I wanna peep yo' big-ass daddy meat~ wigglez booty I gots a lil itch o3o wags tail can you please git mah itch~ puts paws on yo' chest nyea~ its a seven inch itch rubs yo' chest can you help me pwease squirms pwetty pwease fucked up grill I need ta be punished runs paws down yo' chest n' bites lip like I need ta be punished straight-up good~ paws on yo' bulge as I lick mah lips I be gettin thirsty. I can go fo' some gin n juice unbuttons yo' baggy-ass pants as mah eyes glow you smell so musky :v licks shaft mmmm~ so musky drools all over yo' ding-a-ling yo' daddy meat I wanna bust a nut on fondlez Mista Muthafuckin Fuzzy Balls hehe puts snout on balls n' inhalez deeply oh god im so hard~ licks balls punish me daddy~ nyea~ squirms mo' n' wigglez booty I gots a straight-up boner fo' yo' musky goodnizz bites lip please punish me licks lips nyea~ sucklez on yo' tip so phat licks pre of yo' ding-a-ling salty goodness~ eyes role back n' goes balls deep mmmm~ moans n' suckles",
                    "navy seals" => "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.",
                    "hit or miss" => "To hit, or not to hit. Dost thou ever miss? I suppose it not. You have a male love interest, yet I would wager he does not kiss thee (Ye olde mwah). Furthermore; he will find another lass like he won't miss thee. And at the end of it all. He is going to skrrt, and he will hit that dab, as if he were the man known by the name of Wiz Khalifa",
                    _ => "I didn't recognise that.",
                };
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} {copypasta}");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} no argument passed in");
            }
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
                sb.AppendLine($"{(region.IsOptimal || region.Id == currentRegion ? Format.Bold(region.Name) : region.Name)} {(region.IsOptimal ? "(Optimal)" : "")} {(region.Id == currentRegion ? "(Current)" : "")}");
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
            // TODO: Create a service for the Alarm entity and do get / creation / update / delete there
            await Task.Delay(2);
            throw new NotImplementedException();
        }

        [Command("display-alarms")]
        [Summary("Display set alarms")]
        [NotImplemented]
        public async Task GetAlarmAsync()
        {
            // TODO: Create a service for the Alarm entity and do get / creation / update / delete there
            await Task.Delay(2);
            throw new NotImplementedException();
        }

        [Command("remove-alarm")]
        [Summary("Remove a set alarm")]
        [NotImplemented]
        public async Task RemoveAlarmAsync()
        {
            // TODO: Create a service for the Alarm entity and do creation / update / delete there
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
        public async Task AddQuoteAsync(string author = "", [Remainder]string quote = "")
        {
            ulong id = Context.Guild.DefaultChannel.Id;
            quote = Regex.Unescape(quote);

            await Context.Guild.GetTextChannel(id).SendMessageAsync($"\"{quote}\" - {author} {DateTime.Now.Year}");

            string fileName = $"{Context.Guild.Name}-{Context.Guild.GetChannel(id).Name}-backup.csv";

            Bunch<CSVColumns> columns = new Bunch<CSVColumns>()
            {
                new CSVColumns
                {
                    MessageID = Context.Message.Id,
                    Author = author,
                    MessageContent = quote,
                    Timestamp = Context.Message.Timestamp
                }
            };

            fileService.WriteToCSV(columns, id, fileName);

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
            ListResponse<string> response = await fileService.ReadFromFileAsync($"{Context.Guild.Name}\\Yotta.txt");
            Bunch<string> yotta = response.Entities.ToBunch();

            Array enumValues = Enum.GetValues(typeof(PrependEnum));
            Bunch<YottaDTO> valueCount = new Bunch<YottaDTO>(yotta.Count());

            foreach (object item in enumValues)
            {
                int prependCount = yotta.Where(x => x.ToString() == item.ToString()).Count();

                valueCount.Add(new YottaDTO { PrependValue = (PrependEnum)item, Count = prependCount });
            }

            StringBuilder sb = new StringBuilder();
            foreach (YottaDTO item in valueCount)
            {
                sb.Append($"{item.PrependValue}: {item.Count}\n");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("yotta-ordered")]
        [Summary("Reads the Yotta count and gives an ordered representation of it")]
        public async Task YottaOrderedAsync()
        {
            ListResponse<string> response = await fileService.ReadFromFileAsync("Yotta.txt");
            Bunch<string> yotta = response.Entities.ToBunch();

            IOrderedEnumerable<string> orderedYotta = yotta.OrderBy(x => x);
            await Context.Channel.SendMessageAsync(string.Join(", ", orderedYotta));
        }

        [Command("emoji-usage")]
        [Summary("Gets the usage of a given emoji in the guild")]
        public async Task EmojiUsageAsync(string emoteName)
        {
            Emote.TryParse($":{emoteName}:", out Emote emote);
            await EmoteUsage(emote);
        }

        [Command("emoji-usage")]
        [Summary("Gets the usage of a given emoji in the guild")]
        public async Task EmojiUsageAsync(ulong emojiId)
        {
            Response<GuildEmote> emote = await coreService.GetEmoteFromGuild(emojiId, Context.Guild);
            await EmoteUsage(emote.Entity);
        }

        private async Task EmoteUsage(Emote emote)
        {
            if (emote != null)
            {
                ListResponse<IMessage> messages = await coreService.GetAllMessagesAsync(Context.Channel.Id);
                Regex emoteRegex = new Regex(@"\<(\:.*?(\b" + emote.Name.ToLowerInvariant() + @"\b)\:)(.*?\d)\>", RegexOptions.IgnoreCase);

                Bunch<IMessage> filteredMessages = messages.Entities.Where(m => emoteRegex.Match(m.Content).Success).ToBunch();

                await Context.Channel.SendMessageAsync($":{emote.Name}: has been used {filteredMessages.Count()} times in {messages.Entities.Count()} messages ({Context.Channel.Name})");
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
        [NotImplemented]
        public async Task ChangeIconAsync()
        {
            await Task.Delay(2);
            throw new NotImplementedException();
        }

        [Command("ping-server")]
        [Summary("Pings a server with a given name or address")]
        [NotImplemented]
        public async Task PingServer(string ipAddress)
        {
            Response<IPStatus> status = await coreService.PingHostAsync(ipAddress);
            await Context.Channel.SendMessageAsync($"{ipAddress} responded with status {status.Entity}");
        }
    }
}
