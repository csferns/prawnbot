using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace prawnbotWF
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

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                DiscordBot bot = new DiscordBot();
                await bot.Client_Log(new LogMessage(LogSeverity.Info, "JoinAudio", $"Connected to voice on {guild.Name}."));
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                DiscordBot bot = new DiscordBot();
                await bot.Client_Log(new LogMessage(LogSeverity.Info, "LeaveAudio", $"Disconnected from voice on {guild.Name}."));
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
                DiscordBot bot = new DiscordBot();
                await bot.Client_Log(new LogMessage(LogSeverity.Debug, "SendAudio", $"Starting playback of {path} in {guild.Name}"));

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
}
