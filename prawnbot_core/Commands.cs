using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly Helpers helpers;
        private readonly AudioService _service;

        public Commands()
        {
            helpers = new Helpers(Context);
            _service = new AudioService();
        }

        public async static Task MemeOClock(DiscordSocketClient Client)
        {
            if (
                (DateTime.Now.Hour == 9 || DateTime.Now.Hour == 21) 
                && DateTime.Now.Minute == 11 
                && DateTime.Now.Second == 0 
                && DateTime.Now.Millisecond >= 200
               )
            {
                foreach (var guild in Client.Guilds)
                {
                    await guild.DefaultChannel.SendMessageAsync("Happy meme o'clock!");
                }

                return;
            }
        } 

        [Command("ping")]
        public async Task PingAsync()
        {
            await  Context.Channel.SendMessageAsync($"{Context.Client.Latency}ms");
        }

        [Command("commands")]
        [Summary("PM's a list of commands to the user")]
        public async Task CommandsAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands | All commands follow the structure p!(command)")
                .WithColor(Color.Blue)
                .AddField("p!daddy", "Sends a message directed at the user calling them the bot's daddy")
                .AddField("p!random-quote", "Sends a random quote from the quote room")
                .AddField("p!status", "Gives the status of the server")
                .AddField("p!user-joined (user)", "Gives a response of when the user joined the server")
                .AddField("p!user-created (user)", "Gives a response of when the user was created")
                .AddField("p!copypasta (copypasta name)", "Sends the given copypasta")
                .AddField("p!kowalski", "Tags James and asks for Analysis")
                .AddField("p!current-region", "Gets the current region of the server")
                .AddField("p!one last ride", "Sends the 'One Last Ride' image")
                .AddField("p!cam murray", "Sends the 'Cam Murray' image")
                .AddField("p!pipe down", "Sends the 'Pipe down' gif")
                .AddField("p!gold elims", "Sends the 'Gold Elims' image")
                .AddField("p!backup (optional: id)", "Backs up quotes in a channel to a csv file")
                .AddField("p!addquote (author) (year) (quote)", "Adds a quote to the quote room");

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
                $"Bot owner: <@!{botInfo.Owner.Id}> \n" +
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
                .WithAuthor(Context.Client.CurrentUser)
                .WithColor(Color.Blue)
                .WithDescription(
                $"The default channel is: {Context.Guild.DefaultChannel} \n" +
                $"The server was created on {Context.Guild.CreatedAt.LocalDateTime} \n" +
                $"The server currently has {Context.Guild.MemberCount} members \n" +
                $"The current AFK Channel is {Context.Guild.AFKChannel} with a timeout of {Context.Guild.AFKTimeout}\n " +
                $"There are currently {Context.Guild.Channels.Count} channels in the server\n " +
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
                string copypasta = null;
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
        public async Task CurrentRegionAsync() => await Context.Channel.SendMessageAsync($"The server is currently located in: **{Context.Client.VoiceRegions.FirstOrDefault(x => x.Id == Context.Guild.VoiceRegionId)}**");

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
                if (region.IsOptimal || region.Id == currentRegion) sb.Append("**");
                sb.Append($"{region.Name}");
                if (region.IsOptimal) sb.Append(" (Optimal)");
                if (region.Id == currentRegion) sb.Append(" (Current)");

                if (region.IsOptimal || region.Id == currentRegion) sb.Append("**");
                sb.Append("\n");
            }

            sb.Append("\n");
            sb.Append("__Deprecated regions:__ \n");

            foreach (var region in allRegions.Where(x => x.IsDeprecated && !x.IsVip))
            {
                if (region.IsOptimal || region.Id == currentRegion) sb.Append("**");

                sb.Append($"{region.Name}");
                if (region.Id == currentRegion) sb.Append(" (Current)");

                if (region.IsOptimal || region.Id == currentRegion) sb.Append("**");
                sb.Append("\n");
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

            await Context.Channel.SendMessageAsync($"Setting server **{Context.Guild}**'s region to {region}");

            var optionalRegion = new Optional<IVoiceRegion>(region);

            await Context.Guild.ModifyAsync(x => x.Region = optionalRegion);
        }

        [Command("impersonate-user")]
        public async Task ImpersonateUser(SocketGuildUser user)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("No user passed in!");
                return;
            }

            RequestOptions options = new RequestOptions
            {
                 RetryMode = RetryMode.RetryTimeouts,
                 Timeout = 1000,
                 CancelToken = CancellationToken.None
            };

            var webhooks = await Context.Guild.GetWebhooksAsync();

            var prawnbotWebhook = webhooks.FirstOrDefault(x => x.Name == "PrawnBot");

            if (prawnbotWebhook != default(RestWebhook))
            {
                //RestWebhook restClient = new RestWebhook(prawnbotWebhook);
            }
        }

        [Command("random-user")]
        [Summary("Gets a random user from the guild and posts them")]
        public async Task RandomUserAsync()
        {
            Random random = new Random();

            var availableUsers = Context.Guild.Users.Where(x => !x.IsBot).ToList();
            var randomUser = availableUsers[random.Next(availableUsers.Count())];

            await Context.Channel.SendMessageAsync(randomUser.Nickname ?? randomUser.Username);
        }
    }
}
