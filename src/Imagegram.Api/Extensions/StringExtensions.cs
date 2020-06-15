namespace Imagegram.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ConvertToUrlPath(this string source)
        {
            return source.Replace('\\', '/');
        }
    }
}