namespace Imagegram.Api
{
    public class PostOptions
    {
        public string SupportedImageFormats { get; set; }

        public int MaxFileSizeKb { get; set; }

        public int FetchCommentsLimit { get; set; }
    }
}