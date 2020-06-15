namespace Imagegram.Api
{
    public static class Constants
    {
        public static class Api
        {
            public const string Name = "Imagegram API";
            public const string Version = "v1";
        }

        public static class Authentication
        {
            public const string HeaderName = "X-Account-Id";
            public const string HeaderBasedSchemeName = "HeaderBased";
            public const string SecuritySchemeName = "apiKey";
        }
    }
}