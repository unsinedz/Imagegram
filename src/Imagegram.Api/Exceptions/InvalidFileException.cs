using System;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Api.Exceptions
{
    public class InvalidFileException : Exception, IStatusCodeException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public InvalidFileException()
        {
        }

        public InvalidFileException(string message) : base(message)
        {
        }

        public InvalidFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}