using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Prawnbot.Core.Module
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("one last ride")]
        public async Task OneLastRideAsync()
        {
            var imageUrl = await _fileService.GetUrlFromBlobStore("one_last_ride.png", "botimages");
            await Context.Channel.SendFileAsync(imageUrl.Entity, "One last ride?");
        }

        [Command("taps head")]
        public async Task TapsHeadAsync()
        {
            await Context.Message.AddReactionAsync(new Emoji("👍"));
            var imageUrl = await _fileService.GetUrlFromBlobStore("james_tapping_head.png", "botimages");
            await Context.Channel.SendFileAsync(imageUrl.Entity, "*taps head*");
        }

        [Command("cam murray")]
        public async Task CamMurrayAsync()
        {
            var imageUrl = await _fileService.GetUrlFromBlobStore("cam_murray.png", "botimages");
            await Context.Channel.SendFileAsync(imageUrl.Entity);
        }

        [Command("gold elims")]
        public async Task GoldElimsAsync()
        {
            var imageUrl = await _fileService.GetUrlFromBlobStore("top_elims.png", "botimages");
            await Context.Channel.SendFileAsync(imageUrl.Entity);
        }
    }
}
