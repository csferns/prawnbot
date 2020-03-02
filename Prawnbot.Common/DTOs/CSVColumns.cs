using System;

namespace Prawnbot.Common.DTOs
{
    public class CSVColumns
    {
        public ulong MessageID { get; set; }
        public string MessageSource { get; set; }
        public string Author { get; set; }
        public string MessageContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int AttachmentCount { get; set; }
        public string Attachments { get; set; }
        public int ReactionCount { get; set; }
        public bool IsPinned { get; set; }
        public bool WasSentByBot { get; set; }
    }
}
