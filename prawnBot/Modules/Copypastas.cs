using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace prawnBot.Modules
{
    public class Copypastas : ModuleBase<SocketCommandContext>
    {
        [Command("copypasta")]
        public async Task OwOAsync(string copypastaName)
        {
            switch(copypastaName)
            {
                default:

                    break;

                case "owo":
                    string copypasta = "Rawr x3 nuzzlez how tha fuck is yo dirty ass pounces on you you so warm o3o notices you gotz a funky-ass bulge o: one of mah thugss aiiight :wink: nuzzlez yo' necky wecky~ murr~ hehehe rubbies yo' bulgy wolgy you so big-ass :oooo rubbies mo' on yo' bulgy wolgy it don't stop growin .///. kisses you n' lickies yo' necky daddy likies (; nuzzlez wuzzlez I hope daddy straight-up likes $: wigglez booty n' squirms I wanna peep yo' big-ass daddy meat~ wigglez booty I gots a lil itch o3o wags tail can you please git mah itch~ puts paws on yo' chest nyea~ its a seven inch itch rubs yo' chest can you help me pwease squirms pwetty pwease fucked up grill I need ta be punished runs paws down yo' chest n' bites lip like I need ta be punished straight-up good~ paws on yo' bulge as I lick mah lips I be gettin thirsty. I can go fo' some gin n juice unbuttons yo' baggy-ass pants as mah eyes glow you smell so musky :v licks shaft mmmm~ so musky drools all over yo' ding-a-ling yo' daddy meat I wanna bust a nut on fondlez Mista Muthafuckin Fuzzy Balls hehe puts snout on balls n' inhalez deeply oh god im so hard~ licks balls punish me daddy~ nyea~ squirms mo' n' wigglez booty I gots a straight-up boner fo' yo' musky goodnizz bites lip please punish me licks lips nyea~ sucklez on yo' tip so phat licks pre of yo' ding-a-ling salty goodness~ eyes role back n' goes balls deep mmmm~ moans n' suckles";
                    await ReplyAsync($"{Context.User.Mention} {copypasta}");
                    break;
            }
        }
    }
}
