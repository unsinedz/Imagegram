using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imagegram.Api.Authentication
{
    public class HeaderBasedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAccountRepository accountRepository;

        public HeaderBasedAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IAccountRepository accountRepository,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this.accountRepository = accountRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerName = Constants.Authorization.HeaderName;
            if (!Request.Headers.TryGetValue(headerName, out var accountIdValue))
                return AuthenticateResult.Fail($"Missing {headerName} header.");

            if (!Guid.TryParse(accountIdValue.ToString(), out var accountId))
                return AuthenticateResult.Fail($"Invalid {headerName} header value.");

            var existingAccount = (await accountRepository.GetAsync(accountId)).SingleOrDefault();
            if (existingAccount == null)
                return AuthenticateResult.Fail($"Account does not exist.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}