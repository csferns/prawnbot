namespace Prawnbot.Core.Model.API.Giphy
{
    public class GiphyRootobject
    {
        public GiphyDatum[] data { get; set; }
        public Pagination pagination { get; set; }
        public Meta meta { get; set; }
    }
}
