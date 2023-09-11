using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Services;

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

    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
    private readonly ISettingsService _settingsService;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ISettingsService settingsService, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
        _settingsService = settingsService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Setting setting = await _settingsService.GetSettingsAsync();
        bool needsAuth = setting.AuthenticationMethod != AuthenticationType.None;

        if (!needsAuth)
        {
            List<Claim> claims = new()
            {
                    new Claim("ApiKey", "true")
                };

            ClaimsIdentity identity = new(claims, Options.AuthenticationType);
            List<ClaimsIdentity> identities = new() { identity };
            ClaimsPrincipal principal = new(identities);
            AuthenticationTicket ticket = new(principal, Options.Scheme);
            _logger.LogDebug("Authentication is off");
            return AuthenticateResult.Success(ticket);
        }

        string? providedApiKey = ParseApiKey(setting.ApiKey, setting.ServerKey);

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            _logger.LogDebug("No Authentication: providedApiKey is blank");
            return AuthenticateResult.NoResult();
        }

        if (setting.ApiKey == providedApiKey)
        {
            List<Claim> claims = new()
            {
                    new Claim("ApiKey", "true")
                };

            ClaimsIdentity identity = new(claims, Options.AuthenticationType);
            List<ClaimsIdentity> identities = new() { identity };
            ClaimsPrincipal principal = new(identities);
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

    private string? ParseApiKey(string apiKey, string serverKey)
    {
        if (Request.Path.Value.StartsWith("/swagger") && Debugger.IsAttached)
        {
            _logger.LogDebug("Swagger Authentication success");
            return apiKey;
        }
        string requestPath = Context.Request.Path.Value.ToString();

        if (Options.QueryName == "SGLinks")
        {

            _logger.LogDebug("SGLinks Authentication start for {requestPath}", requestPath);
            // Get the request path            
            if (
                !requestPath.StartsWith("/api/videostreams/", StringComparison.InvariantCultureIgnoreCase)
                &&
                !requestPath.StartsWith("/api/streamgroups/", StringComparison.InvariantCultureIgnoreCase)
                )
            {
                _logger.LogDebug("SGLinks: Bad path No Authentication for {requestPath}", requestPath);
                return null;
            }

            string crypt = requestPath.GetAPIKeyFromPath(serverKey);
            if (string.IsNullOrEmpty(crypt))
            {
                _logger.LogDebug("SGLinks: crypt is blank for {requestPath}", requestPath);
            }

            return crypt;
        }
        _logger.LogDebug("Authentication start for {requestPath}", requestPath);
        // Try query parameter
        if (Request.Query.TryGetValue(Options.QueryName, out Microsoft.Extensions.Primitives.StringValues value))
        {
            _logger.LogDebug("Authentication used query parameter {value}", value.FirstOrDefault());
            return value.FirstOrDefault();
        }

        // No ApiKey query parameter found try headers
        if (Request.Headers.TryGetValue(Options.HeaderName, out Microsoft.Extensions.Primitives.StringValues headerValue))
        {
            _logger.LogDebug("Authentication used headers {headerName} : {value}", Options.HeaderName, headerValue.FirstOrDefault());
            return headerValue.FirstOrDefault();
        }

        _logger.LogDebug("Authentication used bearer : {value}", Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", ""));
        return Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
    }
}