using System;
using System.Security.Claims;

namespace Imagegram.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return Guid.Parse("2DA06B5B-CAEC-4E31-8396-3960F0ABB3F4");
            return Guid.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}