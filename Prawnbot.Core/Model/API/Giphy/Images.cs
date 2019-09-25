namespace Prawnbot.Core.Model.API.Giphy
{
    public class Images
    {
        public Fixed_Height_Still fixed_height_still { get; set; }
        public Original_Still original_still { get; set; }
        public Fixed_Width fixed_width { get; set; }
        public Fixed_Height_Small_Still fixed_height_small_still { get; set; }
        public Fixed_Height_Downsampled fixed_height_downsampled { get; set; }
        public Preview preview { get; set; }
        public Fixed_Height_Small fixed_height_small { get; set; }
        public Downsized_Still downsized_still { get; set; }
        public Downsized downsized { get; set; }
        public Downsized_Large downsized_large { get; set; }
        public Fixed_Width_Small_Still fixed_width_small_still { get; set; }
        public Preview_Webp preview_webp { get; set; }
        public Fixed_Width_Still fixed_width_still { get; set; }
        public Fixed_Width_Small fixed_width_small { get; set; }
        public Downsized_Small downsized_small { get; set; }
        public Fixed_Width_Downsampled fixed_width_downsampled { get; set; }
        public Downsized_Medium downsized_medium { get; set; }
        public Original original { get; set; }
        public Fixed_Height fixed_height { get; set; }
        public Looping looping { get; set; }
        public Original_Mp4 original_mp4 { get; set; }
        public Preview_Gif preview_gif { get; set; }
        public _480W_Still _480w_still { get; set; }
        public Hd hd { get; set; }
    }

    public class Fixed_Height_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Original_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Fixed_Width
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Fixed_Height_Small_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Fixed_Height_Downsampled
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Preview
    {
        public string width { get; set; }
        public string height { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
    }

    public class Fixed_Height_Small
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Downsized_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Downsized
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Downsized_Large
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Fixed_Width_Small_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Preview_Webp
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Fixed_Width_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Fixed_Width_Small
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Downsized_Small
    {
        public string width { get; set; }
        public string height { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
    }

    public class Fixed_Width_Downsampled
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Downsized_Medium
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class Original
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string frames { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
        public string hash { get; set; }
    }

    public class Fixed_Height
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
        public string webp { get; set; }
        public string webp_size { get; set; }
    }

    public class Looping
    {
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
    }

    public class Original_Mp4
    {
        public string width { get; set; }
        public string height { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
    }

    public class Preview_Gif
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
    }

    public class _480W_Still
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class Hd
    {
        public string width { get; set; }
        public string height { get; set; }
        public string mp4 { get; set; }
        public string mp4_size { get; set; }
    }
}
