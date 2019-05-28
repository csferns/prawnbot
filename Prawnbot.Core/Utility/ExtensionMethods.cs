using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prawnbot.Core.Utility
{
    public static class ExtensionMethods
    {
        public static bool ContainsInsideWord(this string message, string textToFind)
        {
            return message.Contains(textToFind);
        }

        public static bool ContainsSingleLower(this string message, string textToFind)
        {
            string[] splitMessage = message.ToLowerInvariant().Split(' ');

            textToFind = textToFind.ToLowerInvariant();

            foreach (var word in splitMessage)
            {
                if (word == textToFind)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsManyLower(this string message, string textToFind)
        {
            string[] splitMessage = message.ToLowerInvariant().Split(' ');
            string[] splitTextToFind = textToFind.ToLowerInvariant().Split(' ');

            int foundWords = 0;

            foreach (var item in splitTextToFind)
            {
                if (splitMessage.Contains(item)) foundWords++;
            }

            return foundWords == splitTextToFind.Count();
        }

        public static bool IsUserTagged(this SocketUserMessage message, ulong id)
        {
            return message.Content.Contains($"<@!{id}>");
        }

        public static async Task<string> GetLanguageFullName(this string origin)
        {
            API.APIService apiService = new API.APIService();
            var languages = await apiService.GetLanguagesAsync();
            var language = languages.Entities.FirstOrDefault().Languages.SelectMany(x => x.LanguageDetails).Where(y => y.dir == origin).FirstOrDefault().name;

            return language;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9')
                    || (c >= 'A' && c <= 'Z')
                    || (c >= 'a' && c <= 'z')
                    || c == ' ')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string RandomStringFromList(this List<string> list)
        {
            Random random = new Random();
            List<string> newList = new List<string>();

            foreach (string value in list)
            {
                newList.AddRange(Enumerable.Repeat(value, 5));
            }

            return newList[random.Next(newList.Count())];
        } 

        public static Image GetImageFromStream(this Stream stream)
        {
            return new Image(stream);
        }
    }
}
