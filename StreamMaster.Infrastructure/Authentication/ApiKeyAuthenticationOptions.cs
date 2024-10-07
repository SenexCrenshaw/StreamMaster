using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;

using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StreamMaster.Infrastructure.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "API Key";
    public string AuthenticationType = DefaultScheme;
    public string? HeaderName { get; set; }
    public string? QueryName { get; set; }
    public string Scheme => DefaultScheme;
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    IOptionsMonitor<Setting> intSettings,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
    private static readonly List<string> SafePaths =
    [
        "/api/streamgroups/",
        "/api/videostreams/",
        "/s/",
        "/v/"
    ];

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Setting settings = intSettings.CurrentValue;
        bool needsAuth = settings.AuthenticationMethod != "None";

        if (!needsAuth)
        {
            List<Claim> claims =
            [
                new Claim("ApiKey", "true")
            ];

            ClaimsIdentity identity = new(claims, Options.AuthenticationType);
            ClaimsPrincipal principal = new([identity]);
            AuthenticationTicket ticket = new(principal, Options.Scheme);
            _logger.LogDebug("Authentication is off");
            return AuthenticateResult.Success(ticket);
        }


        string? providedApiKey = await ParseApiKeyAsync(settings.ServerKey);

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            _logger.LogDebug("No Authentication: providedApiKey is blank");
            return AuthenticateResult.Fail("No Authentication: providedApiKey is blank");
        }

        if (settings.ServerKey == providedApiKey)
        {
            List<Claim> claims =
            [
                new Claim("ApiKey", "true")
            ];

            ClaimsIdentity identity = new(claims, Options.AuthenticationType);
            ClaimsPrincipal principal = new([identity]);
            AuthenticationTicket ticket = new(principal, Options.Scheme);
            _logger.LogDebug("ApiKey Authentication success");
            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.NoResult();
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

    private static bool IsSafePath(string requestPath)
    {
        return SafePaths.Any(path => requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<string?> ParseApiKeyAsync(string serverKey)
    {
        if (string.IsNullOrEmpty(Request.Path.Value))
        {
            return null;
        }

        if (Request.Path.Value.StartsWith("/swagger") && Debugger.IsAttached)
        {
            _logger.LogDebug("Swagger Authentication success");
            return serverKey;
        }

        string requestPath = Context.Request.Path.Value ?? string.Empty;

        if (Options.QueryName == "SGLinks")
        {
            _logger.LogDebug("SGLinks Authentication start for {requestPath}", requestPath);

            if (!IsSafePath(requestPath))
            {
                _logger.LogDebug("SGLinks: Bad path No Authentication for {requestPath}", requestPath);
                return null;
            }

            // Example logic to derive crypt value
            string? crypt = serverKey; // Example placeholder: requestPath.GetAPIKeyFromPath(serverKey);
            if (string.IsNullOrEmpty(crypt))
            {
                _logger.LogDebug("SGLinks: crypt is blank for {requestPath}", requestPath);
            }

            return crypt;
        }

        if (Options.QueryName == "SignalR")
        {
            if (!requestPath.StartsWith("/streammasterhub/"))
            {
                _logger.LogDebug("SignalR: Bad path No Authentication for {requestPath}", requestPath);
                return null;
            }

            // Check for a valid Forms authentication cookie
            AuthenticateResult cookieAuthResult = await Context.AuthenticateAsync(nameof(AuthenticationType.Forms));
            if (cookieAuthResult.Succeeded && cookieAuthResult.Principal != null)
            {
                _logger.LogDebug("Authentication success using Forms cookie for {requestPath}", Context.Request.Path);
                return serverKey;
            }
            return null;

        }

        if (Options != null)
        {
            _logger.LogDebug("Authentication start for {requestPath}", requestPath);

            // Try query parameter
            if (Options.QueryName != null && Request.Query.TryGetValue(Options.QueryName, out Microsoft.Extensions.Primitives.StringValues queryValue))
            {
                _logger.LogDebug("Authentication used query parameter {value}", queryValue.FirstOrDefault());
                return queryValue.FirstOrDefault();
            }

            // Try header parameter
            if (Options.HeaderName != null && Request.Headers.TryGetValue(Options.HeaderName, out Microsoft.Extensions.Primitives.StringValues headerValue))
            {
                _logger.LogDebug("Authentication used header {headerName} : {value}", Options.HeaderName, headerValue.FirstOrDefault());
                return headerValue.FirstOrDefault();
            }

            // Try Authorization header
            _logger.LogDebug("Authentication used bearer : {value}", Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", ""));
            return Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        }

        return null;
    }
}