using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMaster.Domain.Enums;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StreamMaster.Infrastructure.Authentication
{
    public class NoAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public NoAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            List<Claim> claims =
            [
                new Claim("user", "Anonymous"),
                new Claim("AuthType", AuthenticationType.None.ToString())
            ];

            ClaimsIdentity identity = new(claims, "NoAuth", "user", "identifier");
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, "NoAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
