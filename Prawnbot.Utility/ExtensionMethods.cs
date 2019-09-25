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

            return splitMessage.Any(x => x.Equals(textToFind.ToLowerInvariant()));
        }

        public static bool ContainsManyLower(this string message, string[] lookupValues)
        {
            return !lookupValues.Except(message.ToLowerInvariant().Split(' ')).Any();
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

        public static bool ContainsEmote(this SocketUserMessage message)
        {
            try
            {
                Regex emoteRegex = new Regex(@"(\<(\:.*?\:)(.*?\d)\>)", RegexOptions.IgnoreCase);
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
                Regex emojiRegex = new Regex(@"\<(\:.*?\:)(.*\d)\>", RegexOptions.IgnoreCase);
                return emojiRegex.Match(message.Content).Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
