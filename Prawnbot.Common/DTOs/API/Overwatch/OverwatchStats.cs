namespace Prawnbot.Common.DTOs.API.Overwatch
{
    public class OverwatchStats 
    {
        public Competitivestats competitiveStats { get; set; }
        public int endorsement { get; set; }
        public string endorsementIcon { get; set; }
        public int gamesWon { get; set; }
        public string icon { get; set; }
        public int level { get; set; }
        public string levelIcon { get; set; }
        public string name { get; set; }
        public int prestige { get; set; }
        public string prestigeIcon { get; set; }
        public bool _private { get; set; }
        public Quickplaystats quickPlayStats { get; set; }
        public int rating { get; set; }
        public string ratingIcon { get; set; }
        public object ratings { get; set; }
    }
}
