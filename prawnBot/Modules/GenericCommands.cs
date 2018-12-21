using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace prawnBot.Modules
{ 
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("commands")]
        public async Task CommandsAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands")
                .WithColor(Color.Blue)
                .WithDescription("All commands follow the structure p!(command)")
                .AddField("Daddy", "Sends a message directed at the user calling them the bot's daddy");

            await ReplyAsync("", false, builder.Build());
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Ping")
                .WithColor(Color.Blue)
                .WithDescription("This is a really nice ping"); 

            await ReplyAsync("", false, builder.Build());
        }

        [Command("daddy")]
        public async Task DaddyAsync()
        {
            await ReplyAsync($"{Context.User.Mention} you can be my daddy if you want :wink:");
        }

        [Command("kys")]
        public async Task KysAsync()
        {
            await ReplyAsync($"Alright {Context.User.Username}, that was very rude. Instead, take your own advice.");
        }

        [Command("kowalski")]
        public async Task AnalysisAsync()
        {
            Emoji penguin = new Emoji("🐧");

            await Context.Message.AddReactionAsync(penguin);
            await ReplyAsync($"<@!147860921488900097> analysis");
        }

        [Command("copypasta")]
        public async Task CopypastaAsync(string copypastaName = "")
        {
            if (copypastaName != "")
            {
                string copypasta = "";

                switch (copypastaName)
                {
                    default:
                        copypasta = "I didn't recognise that.";
                        break;

                    case "owo":
                        copypasta = "Rawr x3 nuzzlez how tha fuck is yo dirty ass pounces on you you so warm o3o notices you gotz a funky-ass bulge o: one of mah thugss aiiight :wink: nuzzlez yo' necky wecky~ murr~ hehehe rubbies yo' bulgy wolgy you so big-ass :oooo rubbies mo' on yo' bulgy wolgy it don't stop growin .///. kisses you n' lickies yo' necky daddy likies (; nuzzlez wuzzlez I hope daddy straight-up likes $: wigglez booty n' squirms I wanna peep yo' big-ass daddy meat~ wigglez booty I gots a lil itch o3o wags tail can you please git mah itch~ puts paws on yo' chest nyea~ its a seven inch itch rubs yo' chest can you help me pwease squirms pwetty pwease fucked up grill I need ta be punished runs paws down yo' chest n' bites lip like I need ta be punished straight-up good~ paws on yo' bulge as I lick mah lips I be gettin thirsty. I can go fo' some gin n juice unbuttons yo' baggy-ass pants as mah eyes glow you smell so musky :v licks shaft mmmm~ so musky drools all over yo' ding-a-ling yo' daddy meat I wanna bust a nut on fondlez Mista Muthafuckin Fuzzy Balls hehe puts snout on balls n' inhalez deeply oh god im so hard~ licks balls punish me daddy~ nyea~ squirms mo' n' wigglez booty I gots a straight-up boner fo' yo' musky goodnizz bites lip please punish me licks lips nyea~ sucklez on yo' tip so phat licks pre of yo' ding-a-ling salty goodness~ eyes role back n' goes balls deep mmmm~ moans n' suckles";
                        break;

                    case "navyseals":
                        copypasta = "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";
                        break;
                }

                await ReplyAsync($"{Context.User.Mention} {copypasta}");
            }
            else
                await ReplyAsync($"{Context.User.Mention} no argument passed in");
        }
    }
}
