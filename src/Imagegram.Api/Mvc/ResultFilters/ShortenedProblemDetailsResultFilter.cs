using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Imagegram.Api.Mvc.ResultFilters
{
    public class ShortenedProblemDetailsResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is ProblemDetails problemDetails)
            {
                var shortened = new ShortenedProblemDetails
                {
                    Status = problemDetails.Status,
                    Title = problemDetails.Title,
                    Detail = problemDetails.Detail
                };
                objectResult.Value = shortened;
            }
        }
    }

    public class ShortenedProblemDetails
    {
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public int? Status { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("detail", NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }
    }
}