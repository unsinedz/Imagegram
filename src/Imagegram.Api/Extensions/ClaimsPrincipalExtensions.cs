using System;
using System.Security.Claims;

namespace Imagegram.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets account ID from the claims.
        /// </summary>
        /// <param name="claimsPrincipal">Current claims principal.</param>
        /// <returns>The account ID if the claim is present.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when the claim is not present (user is not authenticated, etc.).</exception>
        public static Guid GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var claimValue = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(claimValue);
        }
    }
}