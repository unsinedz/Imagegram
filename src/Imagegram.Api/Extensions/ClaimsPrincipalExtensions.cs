using System;
using System.Security.Claims;

namespace Imagegram.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return Guid.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}