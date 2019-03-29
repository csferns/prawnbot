using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace prawnbot_core
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IAudioChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }

            var audioClient = await target.ConnectAsync();
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public async Task CreateStream(Stream stream, string path)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });

            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
        }

        /// <summary>
        /// write example PCM data of a note to a stream
        /// </summary>
        public void GeneratePCM(Stream stream, double frequency, double duration, double volume)
        {
            const int channels = 2; // stereo
            const int sampleRate = 48000; // 48000 samples per sec

            // binary writer uses Little Endian, which is what we need
            var writer = new BinaryWriter(stream);
            int sampleCount = (int)Math.Round(duration * sampleRate);
            for (int t = 0; t < sampleCount; t++)
            {
                for (int c = 0; c < channels; c++)
                {
                    // create sample
                    double sampleValue = Math.Sin(2 * Math.PI * (frequency * t / sampleRate)) * volume;
                    // encode sample in 16 bits
                    short sample16bit = (short)Math.Round(short.MaxValue * sampleValue);
                    writer.Write(sample16bit);
                }
            }
        }
    }

    public partial class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            var audioClient = await channel.ConnectAsync();
            var stream = audioClient.CreatePCMStream(AudioApplication.Music);

            await _service.CreateStream(stream, $"{Environment.CurrentDirectory}\\Audio\\Habits.mp3");

            await stream.FlushAsync();
            await audioClient.StopAsync();
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        [Command("asmr", RunMode = RunMode.Async)]
        public async Task AsmrAsync()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, "D:\\Libraries\\Music\\Youtube to mp3\\All star but they don't stop coming.mp3");
        }
    }
}
