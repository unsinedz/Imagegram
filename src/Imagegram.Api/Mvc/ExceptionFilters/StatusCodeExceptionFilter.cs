using Imagegram.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Imagegram.Api.Mvc.ExceptionFilters
{
    public class StatusCodeExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is IStatusCodeException statusCodeException)
            {
                context.Result = new StatusCodeResult(statusCodeException.StatusCode);
                context.ExceptionHandled = true;
            }
        }
    }
}