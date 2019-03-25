using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prawnbotWF
{
    public static class ExtensionMethods
    {
        public static bool ContainsLower(this SocketUserMessage message, string textToFind)
        {
            if (message.Content.ToLower().Contains(textToFind.ToLower())) return true;
            else return false;
        }
    }
}
