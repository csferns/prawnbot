using CsvHelper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Base;
using Prawnbot.Core.Models;
using Prawnbot.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Module
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        protected LocalFileAccess.IFileService _fileService;
        protected Command.ICommandService _commandService;
        protected API.IAPIService _apiService;
        protected Bot.IBotService _botService;

        public Modules()
        {
            _fileService = new LocalFileAccess.FileService();
            _commandService = new Command.CommandService();
            _apiService = new API.APIService();
            _botService = new Bot.BotService(Context);
        }

        [Command("random-user")]
        [Summary("Gets a random user from the guild and posts them")]
        public async Task RandomUserAsync()
        {
            Random random = new Random();

            var availableUsers = _botService.GetAllUsers();
            var randomUser = availableUsers.Entities[random.Next(availableUsers.Entities.Count())];

            await Context.Channel.SendMessageAsync(randomUser.Nickname ?? randomUser.Username);
        }

        [Command("flip a coin")]
        [Summary("Flips a coin")]
        public async Task FlipACoinAsync(string headsValue = null, string tailsValue = null)
        {
            var response = _commandService.FlipACoin(headsValue, tailsValue).Entity;

            await Context.Channel.SendMessageAsync("Flipping coin...");
            await Task.Delay(1000);
            await Context.Channel.SendMessageAsync(response);
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync($"{Context.Client.Latency}ms");
        }

        [Command("commands")]
        [Summary("PM's a list of commands to the user")]
        public async Task CommandsAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            CommandAttribute[] commandAttributes = (CommandAttribute[])Attribute.GetCustomAttributes(typeof(Modules).Assembly, typeof(CommandAttribute));
            object[] summaryAttribues = typeof(SummaryAttribute).GetCustomAttributes(true);

            var attributes = commandAttributes.Zip(summaryAttribues, (cmd, smr) => new { Command = cmd, Summary = smr });

            builder.WithTitle("Commands | All commands follow the structure p!(command)")
                .WithColor(Color.Blue);

            foreach (var attribute in attributes) 
            {
                builder.AddField(attribute.Command.ToString(), attribute.Summary.ToString());
            }

            await Context.User.SendMessageAsync("", false, builder.Build());
            await Context.Channel.SendMessageAsync($"{Context.User.Mention}: pm'd with command details!");
        }

        [Command("info")]
        [Summary("Gets the bot's information")]
        public async Task GetBotInfoAsync()
        {
            var botInfo = await Context.Client.GetApplicationInfoAsync();

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle(botInfo.Name)
                .WithColor(Color.Blue)
                .WithDescription(
                $"Bot owner: {botInfo.Owner.Username} \n" +
                $"Created at: {botInfo.CreatedAt}\n" +
                $"Description: {botInfo.Description}"
                );

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("status")]
        [Summary("Gives the status of the server")]
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

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("copypasta")]
        [Summary("Gives a copypasta back to the user")]
        public async Task CopypastaAsync([Remainder]string copypastaName)
        {
            if (copypastaName != "")
            {
                string copypasta = string.Empty;
                switch (copypastaName.ToLower())
                {
                    default:
                        copypasta = "I didn't recognise that.";
                        break;

                    case "owo":
                        copypasta = "Rawr x3 nuzzlez how tha fuck is yo dirty ass pounces on you you so warm o3o notices you gotz a funky-ass bulge o: one of mah thugss aiiight :wink: nuzzlez yo' necky wecky~ murr~ hehehe rubbies yo' bulgy wolgy you so big-ass :oooo rubbies mo' on yo' bulgy wolgy it don't stop growin .///. kisses you n' lickies yo' necky daddy likies (; nuzzlez wuzzlez I hope daddy straight-up likes $: wigglez booty n' squirms I wanna peep yo' big-ass daddy meat~ wigglez booty I gots a lil itch o3o wags tail can you please git mah itch~ puts paws on yo' chest nyea~ its a seven inch itch rubs yo' chest can you help me pwease squirms pwetty pwease fucked up grill I need ta be punished runs paws down yo' chest n' bites lip like I need ta be punished straight-up good~ paws on yo' bulge as I lick mah lips I be gettin thirsty. I can go fo' some gin n juice unbuttons yo' baggy-ass pants as mah eyes glow you smell so musky :v licks shaft mmmm~ so musky drools all over yo' ding-a-ling yo' daddy meat I wanna bust a nut on fondlez Mista Muthafuckin Fuzzy Balls hehe puts snout on balls n' inhalez deeply oh god im so hard~ licks balls punish me daddy~ nyea~ squirms mo' n' wigglez booty I gots a straight-up boner fo' yo' musky goodnizz bites lip please punish me licks lips nyea~ sucklez on yo' tip so phat licks pre of yo' ding-a-ling salty goodness~ eyes role back n' goes balls deep mmmm~ moans n' suckles";
                        break;

                    case "navy seals":
                        copypasta = "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";
                        break;
                    case "hit or miss":
                        copypasta = "To hit, or not to hit. Dost thou ever miss? I suppose it not. You have a male love interest, yet I would wager he does not kiss thee (Ye olde mwah). Furthermore; he will find another lass like he won't miss thee. And at the end of it all. He is going to skrrt, and he will hit that dab, as if he were the man known by the name of Wiz Khalifa";
                        break;
                }

                await Context.Channel.SendMessageAsync($"{Context.User.Mention} {copypasta}");
            }
            else
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} no argument passed in");
        }

        [Command("user-joined")]
        [Summary("Gets when the user joined the server")]
        public async Task GetUserJoinedAsync(SocketGuildUser user) => await Context.Channel.SendMessageAsync($"User {user.Mention} joined at {user.JoinedAt}");

        [Command("user-created")]
        [Summary("Gets when the user joined discord")]
        public async Task GetUserCreatedAsync(SocketGuildUser user) => await Context.Channel.SendMessageAsync($"User {user.Mention} was created on {user.CreatedAt.ToLocalTime()}");

        [Command("current-region")]
        [Summary("Gets the server's current region")]
        public async Task CurrentRegionAsync() => await Context.Channel.SendMessageAsync($"The server is currently located in: {Format.Bold(Context.Client.VoiceRegions.FirstOrDefault(x => x.Id == Context.Guild.VoiceRegionId).Name)}");

        [Command("all-regions")]
        [Summary("Gets all the server regions")]
        public async Task GetAllRegionsAsync()
        {
            var allRegions = Context.Client.VoiceRegions;
            var currentRegion = Context.Guild.VoiceRegionId;

            StringBuilder sb = new StringBuilder();

            sb.Append("__Active regions:__ \n");
            foreach (var region in allRegions.Where(x => !x.IsDeprecated && !x.IsVip))
            {
                sb.Append($"{(region.IsOptimal || region.Id == currentRegion ? Format.Bold(region.Name) : region.Name)}");
                if (region.IsOptimal) sb.Append(" (Optimal)");
                if (region.Id == currentRegion) sb.Append(" (Current)");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.Append("__Deprecated regions:__ \n");

            foreach (var region in allRegions.Where(x => x.IsDeprecated && !x.IsVip))
            {
                sb.Append($"{(region.IsOptimal || region.Id == currentRegion ? Format.Bold(region.Name) : region.Name)}");
                if (region.Id == currentRegion) sb.Append(" (Current)");
                sb.AppendLine();
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("set-region")]
        [Summary("Sets the current region of the server")]
        public async Task SetRegionAsync([Remainder]string regionName = null)
        {
            if (regionName == null)
            {
                await Context.Channel.SendMessageAsync("Region cannot be empty");
                return;
            }

            var regions = await Context.Guild.GetVoiceRegionsAsync();
            var region = Context.Guild.GetVoiceRegionsAsync().ToAsyncEnumerable().FlattenAsync().Result.FirstOrDefault(x => x.Name == regionName);

            bool validRegion = false;

            foreach (var item in regions)
            {
                if (item.Id == region.Id)
                {
                    validRegion = true;
                    break;
                }
            }

            if (!validRegion)
            {
                await Context.Channel.SendMessageAsync($"\"{regionName}\" is not a valid region, or the server cannot access this region.");
                return;
            }

            await Context.Channel.SendMessageAsync($"Setting server {Format.Bold(Context.Guild.Name)}'s region to {region}");

            var optionalRegion = new Optional<IVoiceRegion>(region);

            await Context.Guild.ModifyAsync(x => x.Region = optionalRegion);
        }

        [Command("set-alarm")]
        [Summary("sets an alarm for a user")]
        public async Task SetAlarmAsync(int timePassed, [Remainder]string alarmName = null)
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}";
            string filePath = $"{folderPath}\\alarm.csv";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath);
            }

            DateTime newAlarmTime = DateTime.Now.AddHours(timePassed);

            Alarm alarm = new Alarm()
            {
                AlarmName = alarmName,
                AlarmTime = newAlarmTime,
                User = Context.User.Username
            };

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fileStream))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                fileStream.Position = fileStream.Length;
                csv.WriteRecord(alarm);
            }

            await Context.Channel.SendMessageAsync($"{alarm.AlarmTime} alarm set for {alarm.User}");
        }

        [Command("display-alarms")]
        public async Task GetAlarmAsync()
        {
            string folderPath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}";
            string filePath = $"{folderPath}\\alarm.csv";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath);
                await Context.Channel.SendMessageAsync("No alarms set!");
                return;
            }

            StringBuilder sb = new StringBuilder();

            IEnumerable<Alarm> alarmFile = null;

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            using (CsvReader reader = new CsvReader(streamReader))
            {
                alarmFile = reader.GetRecords<Alarm>();
            }

            if (alarmFile == null)
            {
                await Context.Channel.SendMessageAsync("No alarms set!");
                return;
            }

            foreach (var alarm in alarmFile)
            {
                sb.Append(alarm);
            }
        }

        [Command("remove-alarm")]
        public async Task RemoveAlarmAsync()
        {
            string filePath = $"{Environment.CurrentDirectory}\\Alarms\\{Context.Guild}\\alarm.txt";

            await Task.Delay(200);
        }

        [Command("backup")]
        public async Task BackupQuotesAsync(ulong? id = null)
        {
            await Task.Run(async () =>
            {
                if (Context.Message.Author.Id == 216177905712103424 || Context.Message.Author.Id == 466926694431719424)
                {
                    DateTime startTime = DateTime.Now;
                    if (id.GetValueOrDefault() == default(ulong)) id = Context.Guild.DefaultChannel.Id;

                    await Context.Channel.SendMessageAsync($"Started backup of channel #{Format.Bold(Context.Guild.GetChannel(id.GetValueOrDefault()).Name)} at {startTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

                    var records = await _fileService.CreateCSVList(id.GetValueOrDefault());
                    var result = _fileService.WriteToCSV(records.Entities, id, Context.Guild.Name).Entity;

                    await Context.Channel.SendMessageAsync(result);
                }
            });
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
                    id = Context.Guild.DefaultChannel.Id;
                    break;
            }

            if (quote.Contains("\\")) quote = quote.Replace("\\", @"\");

            await Context.Guild.GetTextChannel(id).SendMessageAsync($"\"{quote}\" - {author} {year}");

            string filePath = $"{Environment.CurrentDirectory}\\ChannelBackups\\{Context.Guild.GetChannel(id).Name}-backup.csv";

            List<CSVColumns> columns = new List<CSVColumns>();

            if (System.IO.File.Exists(filePath))
            {
                CSVColumns recordToAdd = new CSVColumns
                {
                    MessageID = Context.Message.Id,
                    Author = author,
                    MessageContent = quote,
                    Timestamp = Context.Message.Timestamp
                };

                columns.Add(recordToAdd);

                _fileService.WriteToCSV(columns, id, Context.Guild.Name);
            }

            if (Context.Message.Channel.Id == id) await Context.Message.DeleteAsync();
        }

        [Command("randomquote")]
        public async Task RandomQuoteAsync(ulong id = 453899130486521859)
        {
            await Task.Run(async () =>
            {
                if (Context.Message.Author.Id != 258627811844030465)
                {
                    var quoteRoom = await _botService.GetAllMessages(id);
                    Random random = new Random();

                    IMessage randomQuote = quoteRoom.Entities.ElementAt(random.Next(quoteRoom.Entities.Count()));

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
            });
        }

        [Command("backupserver")]
        public async Task BackupServerAsync()
        {
            await Task.Run(async () =>
            {
                DateTime operationTime = DateTime.Now;

                await Context.Channel.SendMessageAsync($"Started backup of server {Context.Guild.Name} ({Context.Guild.TextChannels.Count()} channels) at {operationTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");

                foreach (var textChannel in Context.Guild.TextChannels)
                {
                    DateTime startTime = DateTime.Now;

                    var records = await _fileService.CreateCSVList(textChannel.Id);
                    var result = _fileService.WriteToCSV(records.Entities, textChannel.Id, Context.Guild.Name).Entity;
                    await Context.Channel.SendMessageAsync(result);
                }

                TimeSpan completionTime = DateTime.Now - operationTime;
                await Context.Channel.SendMessageAsync($"Finished server backup of {Context.Guild.Name}. The operation took {completionTime.Hours}h:{completionTime.Minutes}m:{completionTime.Seconds}s:{completionTime.Milliseconds}ms");
            });
        }

        [Command("yotta count")]
        public async Task YottaCountAsync()
        {
            string[] yotta = _fileService.ReadFromFile("Yotta.txt").Entity;

            var enumValues = Enum.GetValues(typeof(PrependEnum));
            List<YottaModel> valueCount = new List<YottaModel>(yotta.Count());

            foreach (var item in enumValues)
            {
                var prependCount = yotta.Where(x => x.ToString() == item.ToString()).Count();

                valueCount.Add(new YottaModel { PrependValue = (PrependEnum)item, Count = prependCount });
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in valueCount)
            {
                sb.Append($"{item.PrependValue}: {item.Count}\n");
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("yotta ordered")]
        public async Task YottaOrderedAsync()
        {
            string[] yotta = _fileService.ReadFromFile("Yotta.txt").Entity;

            var orderedYotta = yotta.OrderBy(x => x);

            await Context.Channel.SendMessageAsync(string.Join(", ", orderedYotta));
        }

        [Command("all-config")]
        public async Task GetAllConfigAsync()
        {
            Dictionary<string, string> configurationValues = _fileService.GetAllConfigurationValues().Entity;

            StringBuilder sb = new StringBuilder();

            foreach (var configItem in configurationValues.OrderBy(x => x.Key))
            {
                if (configItem.Value != null)
                {
                    sb.AppendLine($"{configItem.Key.Remove(0, 13)}: {configItem.Value}");
                }
            }

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("config")]
        public async Task ChangeConfigurationAsync(string configurationName, string newConfigurationValue)
        {
            _fileService.SetConfigurationValue(configurationName, newConfigurationValue);

            await Context.Channel.SendMessageAsync($"Changed the configuration of {configurationName} to {newConfigurationValue}");
        }
    }
}
