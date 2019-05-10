using CsvHelper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Common.Enums;
using Prawnbot.Core.API;
using Prawnbot.Core.Base;
using Prawnbot.Core.Bot;
using Prawnbot.Core.Models;
using Prawnbot.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Command
{
    public interface ICommandBl
    {
        Task<bool> ContainsUser(SocketCommandContext context, SocketUserMessage message);
        Task<bool> ContainsText(SocketCommandContext context, SocketUserMessage message);
        string TagUser(ulong id);
        string FlipACoin(string headsValue, string tailsValue);
        Task<string[]> YottaPrepend(SocketGuild guild);

    }

    public class CommandBl : BaseBl, ICommandBl
    {
        protected IAPIBl _apiBl;
        public CommandBl()
        {
            _apiBl = new APIBl();
        }

        public async Task<bool> ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("sam") || message.IsUserTagged(258627811844030465)) await context.Channel.SendMessageAsync("Has the big gay");
            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("ilja") || message.Content.RemoveSpecialCharacters().ContainsSingleLower("ultratwink") || message.IsUserTagged(341940376057282560)) await context.Channel.SendMessageAsync("Has terminal gay");
            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("cam") || message.Content.RemoveSpecialCharacters().ContainsSingleLower("cameron") || message.IsUserTagged(216177905712103424)) await context.Channel.SendMessageAsync("*Father Cammy");

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("sean") || message.IsUserTagged(201371614489608192) || message.Content.RemoveSpecialCharacters().ContainsSingleLower("sean's"))
            {
                string[] yotta = await YottaPrepend(context.Guild);
                string yottaFull = string.Join(", ", yotta);

                await context.Channel.SendMessageAsync($"{yottaFull} {(yottaFull.Length != 0 ? 'c' : 'C')}had Sean, stud of the co-op, leader of the Corfe Mullen massive");
            }

            return true;
        }

        public async Task<bool> ContainsText(SocketCommandContext context, SocketUserMessage message)
        {
            Random random = new Random();

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");
            if (message.Content.RemoveSpecialCharacters().ContainsManyLower("what can i say")) await context.Channel.SendMessageAsync("except, you're welcome!");
            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("africa")) await context.Channel.SendMessageAsync("toto by africa");
            if (message.Content.RemoveSpecialCharacters().ContainsManyLower("oi oi")) await context.Channel.SendMessageAsync("big boi");

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("wheel") || message.Content.RemoveSpecialCharacters().ContainsSingleLower("bus"))
            {
                List<string> randomWheel = new List<string>
                {
                    "The wheels on the bus go round and round",
                    "Excuse me sir, you can't have wheels in this area"
                };

                await context.Channel.SendMessageAsync(randomWheel[random.Next(randomWheel.Count())]);
            }

            if (message.Content.ContainsSingleLower("!skip"))
            {
                var gifs = await _apiBl.GetGifsAsync("what");
                await context.Channel.SendMessageAsync($"you fucking what \n{gifs[random.Next(gifs.Count)].url}");
            }

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("kowalski") || message.Content.RemoveSpecialCharacters().ContainsSingleLower("analysis"))
            {
                var gifs = await _apiBl.GetGifsAsync("kowalski");
                await context.Channel.SendMessageAsync($"{TagUser(147860921488900097)} analysis \n{gifs[random.Next(gifs.Count)].url}");
            }

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("sex"))
            {
                var gifs = await _apiBl.GetGifsAsync("calendar");
                await context.Channel.SendMessageAsync($"Have you put it in the calendar? \n{gifs[random.Next(gifs.Count)].url}");
            }

            if (message.Content.RemoveSpecialCharacters().ContainsManyLower("pipe down"))
            {
                var gifs = await _apiBl.GetGifsAsync("pipe down");
                await context.Channel.SendMessageAsync($"{gifs[0].url}");
            }

            if (message.Content.RemoveSpecialCharacters().ContainsSingleLower("uwu"))
            {
                await context.Message.DeleteAsync();
                await context.Channel.SendMessageAsync("This is an uwu free zone");
            }

            return true;
        }

        public string TagUser(ulong id)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<@!");
            sb.Append(id);
            sb.Append(">");

            return sb.ToString();
        }

        public string FlipACoin(string headsValue, string tailsValue)
        {
            Random random = new Random();
            List<string> coinFlip = new List<string>();

            coinFlip.AddRange(Enumerable.Repeat(headsValue ?? "HEADS", 5));
            coinFlip.AddRange(Enumerable.Repeat(tailsValue ?? "TAILS", 5));

            return coinFlip[random.Next(coinFlip.Count)];
        }

        public async Task<string[]> YottaPrepend(SocketGuild guild)
        {
            Random random = new Random();

            string folderPath = $"{Environment.CurrentDirectory}\\Text Files\\{guild.Name}";
            string filePath = $"{folderPath}\\Yotta.txt";

            var prependEnum = Enum.GetNames(typeof(PrependEnum));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (System.IO.File.Exists(filePath))
            {
                using (StreamWriter file = new StreamWriter(filePath, true))
                {
                    await file.WriteLineAsync(prependEnum[random.Next(prependEnum.Length)]);
                }

                string[] fileLines = System.IO.File.ReadAllLines(filePath);

                return fileLines.Reverse().ToArray();
            }
            else
            {
                System.IO.File.Create(filePath);
                return new string[] { };
            }
        }
    }
}
