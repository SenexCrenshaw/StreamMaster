using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace StreamMaster.Infrastructure.Authentication
{
    public class BasicAuthenticationHandler(IAuthenticationService authService,

        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
    {
        private readonly string _appName = "StreamMaster";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header missing.");
            }

            // Get authorization key
            string authorizationHeader = Request.Headers["Authorization"].ToString();
            Regex authHeaderRegex = new(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return AuthenticateResult.Fail("Authorization code not formatted properly.");
            }

            string authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
            string[] authSplit = authBase64.Split(':', 2);
            string authUsername = authSplit[0];
            string authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

            User? user = authService.Login(Request, authUsername, authPassword);

            if (user == null)
            {
                return AuthenticateResult.Fail("The username or password is not correct.");
            }

            List<Claim> claims =
            [
                new Claim("user", user.Username),
                new Claim("identifier", user.Identifier.ToString()),
                new Claim("AuthType", "Basic")
            ];

            ClaimsIdentity identity = new(claims, "Basic", "user", "identifier");
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, "Basic");

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers.Append("WWW-Authenticate", $"Basic realm=\"{_appName}\"");
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    }
}
