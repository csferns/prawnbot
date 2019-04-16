using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            var audioClient = await channel.ConnectAsync();
            await audioClient.StopAsync();
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }
    }
}
