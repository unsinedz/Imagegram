namespace Imagegram.Api.Exceptions
{
    public interface IStatusCodeException
    {
        int StatusCode { get; }
    }
}