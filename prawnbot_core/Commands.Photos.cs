﻿using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        private string filepath = $"{Environment.CurrentDirectory}\\Photos";

        [Command("one last ride")]
        public async Task OneLastRideAsync() => await Context.Channel.SendFileAsync($@"{filepath}\one_last_ride.png", "One last ride?");

        [Command("taps head")]
        public async Task TapsHeadAsync()
        {
            Emoji thumbsup = new Emoji("👍");

            await Context.Message.AddReactionAsync(thumbsup);
            await Context.Channel.SendFileAsync($@"{filepath}\james_tapping_head.png", "*taps head*");
        }

        [Command("cam murray")]
        public async Task CamMurrayAsync() => await Context.Channel.SendFileAsync($@"{filepath}\cam_murray.png");

        [Command("pipe down")]
        public async Task RetaliateAsync() => await Context.Channel.SendFileAsync($@"{filepath}\pipedown.gif");

        [Command("gold elims")]
        public async Task GoldElimsAsync() => await Context.Channel.SendFileAsync($@"{filepath}\top_elims.png");
    }
}