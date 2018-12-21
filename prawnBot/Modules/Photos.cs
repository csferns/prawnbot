using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace prawnBot.Modules
{
    public class Photos : ModuleBase<SocketCommandContext>
    {
        [Command("one last ride")]
        public async Task OneLastRideAsync()
        {
            await Context.Channel.SendFileAsync("D:\\Projects\\GitHub\\prawnbot\\prawnBot\\Photos\\one_last_ride.png", "One last ride?");
        }

        [Command("taps head")]
        public async Task TapsHeadAsync()
        {
            Emoji thumbsup = new Emoji("👍");

            await Context.Message.AddReactionAsync(thumbsup);
            await Context.Channel.SendFileAsync("D:\\Projects\\GitHub\\prawnbot\\prawnBot\\Photos\\james_tapping_head.png", "*taps head*");
        }

        [Command("cam murray")]
        public async Task CamMurrayAsync()
        {
            await Context.Channel.SendFileAsync("D:\\Projects\\GitHub\\prawnbot\\prawnBot\\Photos\\cam_murray.png");
        }

        [Command("pipe down")]
        public async Task RetaliateAsync()
        {
            await Context.Channel.SendFileAsync("D:\\Projects\\GitHub\\prawnbot\\prawnBot\\Photos\\pipedown.gif");
        }

        [Command("gold elims")]
        public async Task GoldElimsAsync()
        {
            await Context.Channel.SendFileAsync("D:\\Projects\\GitHub\\prawnbot\\prawnBot\\Photos\\top_elims.png");
        }
    }
}
