using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prawnbot.Core.Module
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            IAudioClient audioClient = await channel.ConnectAsync();

            string path = "D:\\Libraries\\Music\\Youtube to mp3\\All Star but they don't stop coming.mp3";

            using (Process ffmpeg = _botService.CreateFfmpegProcess(path).Entity)
            using (AudioOutStream stream = Context.Guild.AudioClient.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }

            await Context.Guild.AudioClient.SetSpeakingAsync(true);
            await audioClient.StopAsync();
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            await _botService.LeaveAudio(Context.Guild).Entity;
        }
    }
}
