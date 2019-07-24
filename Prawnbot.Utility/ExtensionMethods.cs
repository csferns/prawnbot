using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

            return splitMessage.Where(x => x.Equals(textToFind.ToLowerInvariant())).Count() > 0;
        }

        public static bool ContainsManyLower(this string message, string[] lookupValues)
        {
            string[] splitMessage = message.ToLowerInvariant().Split(' ');

            int foundWords = 0;

            foreach (string item in lookupValues)
            {
                if (splitMessage.Equals(item)) foundWords++;
            }

            return foundWords == lookupValues.Count();
        }

        public static bool IsUserTagged(this SocketUserMessage message, ulong id)
        {
            return message.Content.Contains($"<@!{id}>");
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

            return sb.ToString().ToLowerInvariant();
        }

        public static T RandomItemFromList<T>(this ICollection<T> collection)
        {
            Random random = new Random();
            List<T> newList = new List<T>();

            foreach (T value in collection)
            {
                newList.AddRange(Enumerable.Repeat(value, 5));
            }

            return newList[random.Next(newList.Count())];
        }

        public static T RandomOrDefault<T>(this ICollection<T> list)
        {
            if (list.Count() > 0)
            {
                Random random = new Random();
                return list.ElementAtOrDefault<T>(random.Next(list.Count()));
            }

            return default(T);
        }

        public static bool ContainsEmote(this SocketUserMessage message)
        {
            try
            {
                var emoteRegex = new Regex(@"(\<(\:.*?\:)(.*?\d)\>)", RegexOptions.IgnoreCase);
                return emoteRegex.Match(message.Content).Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ContainsEmoji(this SocketUserMessage message)
        {
            try
            {
                var emojiRegex = new Regex(@"\<(\:.*?\:)(.*\d)\>", RegexOptions.IgnoreCase);
                return emojiRegex.Match(message.Content).Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
