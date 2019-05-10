﻿using Discord;
using System;
using System.Collections.Generic;

namespace Prawnbot.Core.Models
{
    public class CSVColumns
    {
        public ulong MessageID { get; set; }
        public string Author { get; set; }
        public bool AuthorIsBot { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Attachment { get; set; }
        public KeyValuePair<IEmote, ReactionMetadata> Reactions { get; set; }
    }
}
