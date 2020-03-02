namespace Prawnbot.Common.DTOs.API.Reddit
{
    public class TopLevelData
    {
        public string modhash { get; set; }
        public int dist { get; set; }
        public Child[] children { get; set; }
        public string after { get; set; }
        public string before { get; set; }
    }
}
