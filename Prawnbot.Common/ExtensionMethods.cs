using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Prawnbot.Common
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

            return splitMessage.Where(x => x.ToLowerInvariant() == textToFind.ToLowerInvariant()).Any();
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

        private static readonly Regex EmoteRegex = new Regex(@"(\<(\:.*?\:)(.*?\d)\>)", RegexOptions.IgnoreCase);

        public static bool ContainsEmote(this SocketUserMessage message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            return EmoteRegex.IsMatch(message.Content);
        }

        private static readonly Regex EmojiRegex = new Regex(@"\p{L}", RegexOptions.IgnoreCase);

        public static bool ContainsEmoji(this SocketUserMessage message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return false;
            }

            return EmojiRegex.IsMatch(message.Content);
        }
    }
}
