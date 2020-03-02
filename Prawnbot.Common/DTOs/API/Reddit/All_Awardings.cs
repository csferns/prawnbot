namespace Prawnbot.Common.DTOs.API.Reddit
{
    public class All_Awardings
    {
        public int count { get; set; }
        public bool is_enabled { get; set; }
        public string subreddit_id { get; set; }
        public string description { get; set; }
        public object end_date { get; set; }
        public int coin_reward { get; set; }
        public string icon_url { get; set; }
        public int days_of_premium { get; set; }
        public string id { get; set; }
        public int icon_height { get; set; }
        public Resized_Icons[] resized_icons { get; set; }
        public int days_of_drip_extension { get; set; }
        public string award_type { get; set; }
        public object start_date { get; set; }
        public int coin_price { get; set; }
        public int icon_width { get; set; }
        public int subreddit_coin_reward { get; set; }
        public string name { get; set; }
    }
}
