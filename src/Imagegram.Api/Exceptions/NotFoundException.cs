using System;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Api.Exceptions
{
    public class NotFoundException : Exception, IStatusCodeException
    {
        public int StatusCode => StatusCodes.Status404NotFound;

        public NotFoundException()
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}