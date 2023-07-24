using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;

using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StreamMasterInfrastructure.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "API Key";
    public string AuthenticationType = DefaultScheme;
    public string HeaderName { get; set; }
    public string QueryName { get; set; }
    public string Scheme => DefaultScheme;
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly string _apiKey;
    private readonly bool needsAuth;
    protected Setting _setting = FileUtil.GetSetting();

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,

        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _apiKey = _setting.ApiKey;
        needsAuth = _setting.AuthenticationMethod != AuthenticationType.None;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!needsAuth)
        {
            var claims = new List<Claim>
                {
                    new Claim("ApiKey", "true")
                };

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        var providedApiKey = ParseApiKey();

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (_apiKey == providedApiKey)
        {
            var claims = new List<Claim>
                {
                    new Claim("ApiKey", "true")
                };

            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 403;
        return Task.CompletedTask;
    }

    private string? ParseApiKey()
    {
        if (Request.Path.Value.StartsWith("/swagger") && Debugger.IsAttached)
        {
            return _apiKey;
        }

        if (Options.QueryName == "SGLinks")
        {
            // Get the request path
            var requestPath = Context.Request.Path.Value.ToString();
            if (!requestPath.StartsWith("/api/videostreams/", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var crypt = requestPath.GetAPIKeyFromPath();

            return crypt;
        }

        // Try query parameter
        if (Request.Query.TryGetValue(Options.QueryName, out var value))
        {
            return value.FirstOrDefault();
        }

        // No ApiKey query parameter found try headers
        if (Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
        {
            return headerValue.FirstOrDefault();
        }

        return Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
    }
}