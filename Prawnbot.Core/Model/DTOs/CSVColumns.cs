using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prawnbot.Core.Model.DTOs
{
    public class CSVColumns
    {
        public ulong MessageID { get; set; }
        public string Author { get; set; }
        public bool AuthorIsBot { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Attachments { get; set; }
        public KeyValuePair<IEmote, ReactionMetadata> Reactions { get; set; }
    }
}
