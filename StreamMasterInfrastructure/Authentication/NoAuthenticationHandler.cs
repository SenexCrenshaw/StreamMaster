using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMasterDomain.Enums;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StreamMasterInfrastructure.Authentication
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
            List<Claim> claims = new()
            {
                new Claim("user", "Anonymous"),
                new Claim("AuthType", AuthenticationType.None.ToString())
            };

            ClaimsIdentity identity = new(claims, "NoAuth", "user", "identifier");
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, "NoAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
