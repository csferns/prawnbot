using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Prawnbot.Common.Enums;
using Prawnbot.Core.Framework;
using Prawnbot.Core.Utility;
using Prawnbot.Data.Models.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.BusinessLayer
{
    public interface ICommandBl
    {
        Task SendImageFromBlobStore(SocketCommandContext context, string fileName);
        Task<bool> ContainsUser(SocketCommandContext context, SocketUserMessage message);
        Task<bool> ContainsText(SocketCommandContext context, SocketUserMessage message);
        string TagUser(ulong id);
        string FlipACoin(string headsValue, string tailsValue);
        Task<string[]> YottaPrepend(SocketGuild guild);
    }

    public class CommandBl : BaseBl, ICommandBl
    {
        public async Task SendImageFromBlobStore(SocketCommandContext context, string fileName)
        {
            Response<Uri> uri = await _fileService.GetUriFromBlobStore(fileName, "botimages");

            WebRequest req = WebRequest.Create(uri.Entity);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                await context.Channel.SendFileAsync(stream, fileName);
            }
        }

        public async Task<bool> ContainsUser(SocketCommandContext context, SocketUserMessage message)
        {
            Random random = new Random();

            string strippedMessage = message.Content.RemoveSpecialCharacters();

            if (strippedMessage.ContainsSingleLower("sam") || message.IsUserTagged(258627811844030465))
            {
                List<GiphyDatum> gifs = await _apiBl.GetGifsAsync("calendar");
                await context.Channel.SendMessageAsync($"Have you put it in the calendar? \n{gifs[random.Next(gifs.Count())].bitly_gif_url}");
            }
            if (strippedMessage.ContainsSingleLower("ilja") || strippedMessage.ContainsSingleLower("ultratwink") || message.IsUserTagged(341940376057282560)) await context.Channel.SendMessageAsync("Has terminal gay");
            if (strippedMessage.ContainsSingleLower("cam") || strippedMessage.ContainsSingleLower("cameron") || message.IsUserTagged(216177905712103424)) await context.Channel.SendMessageAsync("*Father Cammy");

            if (ConfigUtility.YottaMode && (strippedMessage.ContainsSingleLower("sean") || message.IsUserTagged(201371614489608192) || strippedMessage.ContainsSingleLower("seans")))
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

            string strippedMessage = message.Content.RemoveSpecialCharacters();

            if (ConfigUtility.DadMode && strippedMessage.ContainsSingleLower("kys")) await context.Channel.SendMessageAsync($"Alright {context.User.Mention}, that was very rude. Instead, take your own advice.");
            if (ConfigUtility.DadMode && strippedMessage.ContainsSingleLower("dad")) await context.Channel.SendMessageAsync("404 dad not found");

            if (strippedMessage.ContainsSingleLower("daddy")) await context.Channel.SendMessageAsync($"{context.User.Mention} you can be my daddy if you want :wink:");
            if (strippedMessage.ContainsSingleLower("africa")) await context.Channel.SendMessageAsync("toto by africa");
            if (strippedMessage.ContainsSingleLower("big")) await context.Channel.SendMessageAsync("chunky");
            if (strippedMessage.ContainsSingleLower("round")) await context.Channel.SendMessageAsync("plumpy");
            if (strippedMessage.ContainsSingleLower("huge")) await context.Channel.SendMessageAsync("Girl, you huge"); 
            if (strippedMessage.ContainsSingleLower("marvin")) await context.Channel.SendMessageAsync("Marvout");
            if (strippedMessage.ContainsSingleLower("marvout")) await context.Channel.SendMessageAsync("Marvin");
            if (strippedMessage.ContainsSingleLower("engineer")) await context.Channel.SendMessageAsync("The engineer is engihere");
            if (strippedMessage.ContainsSingleLower("ban")) await context.Channel.SendMessageAsync("Did I hear somebody say Macro pad?");
            if (strippedMessage.ContainsSingleLower("2realirl4meirl")) await context.Channel.SendMessageAsync("REEEEEEEEEEEEEEEEEEE");

            if (strippedMessage.ContainsManyLower("what can i say")) await context.Channel.SendMessageAsync("except, you're welcome!");
            if (strippedMessage.ContainsManyLower("oi oi")) await context.Channel.SendMessageAsync("big boi");
            if (strippedMessage.ContainsManyLower("plastic bag")) await context.Channel.SendMessageAsync("Drifting through the wind?");

            if (strippedMessage.ContainsSingleLower("wheel") 
                || strippedMessage.ContainsSingleLower("bus"))
            {
                List<string> randomWheel = new List<string>
                {
                    "The wheels on the bus go round and round",
                    "Excuse me sir, you can't have wheels in this area"
                };

                await context.Channel.SendMessageAsync(randomWheel.RandomStringFromList());
            }

            if (message.Content.ContainsSingleLower("!skip"))
            {
                var gifs = await _apiBl.GetGifsAsync("what");
                await context.Channel.SendMessageAsync($"you fucking what \n{gifs[random.Next(gifs.Count())].bitly_gif_url}");
            }

            if (strippedMessage.ContainsSingleLower("kowalski") 
                || strippedMessage.ContainsSingleLower("analysis"))
            {
                var gifs = await _apiBl.GetGifsAsync("analysis");
                await context.Channel.SendMessageAsync($"{TagUser(147860921488900097)} analysis \n{gifs[random.Next(gifs.Count())].bitly_gif_url}");
            }

            if (strippedMessage.ContainsSingleLower("uwu") 
                || strippedMessage.ContainsManyLower("u w u"))
            {
                await context.Message.DeleteAsync();
                await context.Channel.SendMessageAsync("This is an uwu free zone");
            }

            if (strippedMessage.ContainsManyLower("pipe down"))
            {
                var gifs = await _apiBl.GetGifsAsync("pipe down");
                await context.Channel.SendMessageAsync($"{gifs[random.Next(gifs.Count())].bitly_gif_url}");
            }

            if (strippedMessage.ContainsManyLower("top elims") 
                || strippedMessage.ContainsManyLower("gold elims"))
            {
                await context.Message.AddReactionAsync(new Emoji("👍"));
                await SendImageFromBlobStore(context, "top_elims.png");
            }

            if (strippedMessage.ContainsManyLower("taps head"))
            {
                await context.Message.AddReactionAsync(new Emoji("👍"));
                await SendImageFromBlobStore(context, "james_tapping_head.png");
            }

            if (strippedMessage.ContainsManyLower("one last ride"))
            {
                await context.Message.AddReactionAsync(new Emoji("👍"));
                await SendImageFromBlobStore(context, "one_last_ride.png");
            }

            if (strippedMessage.ContainsManyLower("cam murray"))
            {
                await context.Message.AddReactionAsync(new Emoji("👍"));
                await SendImageFromBlobStore(context, "cam_murray.png");
            }

            if (ConfigUtility.DadMode && strippedMessage.ToLowerInvariant().StartsWith("im"))
            {
                List<string> messageArray = message.Content.ToLowerInvariant().Split(' ').ToList();

                if (messageArray[0].RemoveSpecialCharacters() == "im" && messageArray.Count() != 1)
                {
                    messageArray.RemoveAt(0);

                    await context.Channel.SendMessageAsync($"Hi {string.Join(' ', messageArray.ToList())}, i'm dad");
                }

                return true;
            }

            if (ConfigUtility.ProfanityFilter)
            {
                if (await _apiBl.GetProfanityFilterAsync(strippedMessage))
                {
                    var gifs = await _apiBl.GetGifsAsync("swearing", 50);
                    await context.Channel.SendMessageAsync(gifs[random.Next(gifs.Count())].bitly_gif_url);
                }
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
            List<string> coinFlip = new List<string>
            {
               headsValue ?? "HEADS",
               tailsValue ?? "TAILS"
            };

            return coinFlip.RandomStringFromList();
        }

        public async Task<string[]> YottaPrepend(SocketGuild guild)
        {
            Random random = new Random();

            string[] fileContents = await _fileBl.ReadFromFileAsync($"{guild.Name}\\Yotta.txt");

            Dictionary<string, int> valueDictionary = new Dictionary<string, int>();

            Array enumValues = Enum.GetValues(typeof(PrependEnum));

            foreach (object enumValue in enumValues)
            {
                valueDictionary.Add(enumValue.ToString(), 0);
            }

            foreach (string line in fileContents)
            {
                valueDictionary[line]++;
            }

            bool validValue = false;

            while (!validValue)
            {
                List<string> invalidValues = new List<string>();

                string randomEnumValue = enumValues.GetValue(random.Next(enumValues.Length)).ToString();

                if (valueDictionary[randomEnumValue] < 69)
                {
                    await _fileBl.WriteToFile(randomEnumValue, $"{guild.Name}\\Yotta.txt");
                    fileContents.Append(randomEnumValue);
                    break;
                }
                else
                {
                    invalidValues.Add(randomEnumValue);

                    if (invalidValues.Count == enumValues.Length) break;                
                }
            }

            return fileContents.Reverse().ToArray();
        }
    }
}
