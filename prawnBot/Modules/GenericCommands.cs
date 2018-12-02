using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace prawnBot.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Ping")
                .WithDescription("This is a really nice ping")
                .WithColor(Color.Blue);

            await ReplyAsync("", false, builder.Build());
        }
    }

    public class Daddy : ModuleBase<SocketCommandContext>
    {
        [Command("daddy")]
        public async Task DaddyAsync()
        {
            await ReplyAsync($"{Context.User.Mention} you can be my daddy if you want :wink:");
        }
    }

    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("commands")]
        public async Task CommandsAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Commands")
                .WithColor(Color.Blue)
                .WithDescription("All commands follow the structure prawn!(command)")
                .AddField("Daddy", "Sends a message directed at the user calling them the bot's daddy");

            await ReplyAsync("", false, builder.Build());
        }
    }
}
