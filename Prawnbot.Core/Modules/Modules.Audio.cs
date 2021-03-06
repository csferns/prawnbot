﻿using Discord;
using Discord.Audio;
using Discord.Commands;
using Prawnbot.Core.Attributes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prawnbot.Core.Modules
{
    public partial class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("join-audio", RunMode = RunMode.Async)]
        [Summary("Joins an audio channel")]
        public async Task JoinVoiceChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            using IAudioClient audioClient = await channel.ConnectAsync();

            string path = "D:\\Libraries\\Music\\Youtube to mp3\\All Star but they don't stop coming.mp3";

            using (Process ffmpeg = coreService.CreateFfmpegProcess(path).Entity)
            using (AudioOutStream stream = Context.Guild.AudioClient.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }

            await Context.Guild.AudioClient.SetSpeakingAsync(true);
            await audioClient.StopAsync();
        }

        [Command("leave-audio", RunMode = RunMode.Async)]
        [Summary("Leaves an audio channel")]
        [NotImplemented]
        public async Task LeaveVoiceChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            await coreService.LeaveAudioAsync(Context.Guild);
        }
    }
}
