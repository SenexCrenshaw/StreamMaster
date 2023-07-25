using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;

using StreamMasterInfrastructure.Logging;
using StreamMasterInfrastructure.VideoStreamManager;

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
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
    protected Setting _setting = FileUtil.GetSetting();

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,

        UrlEncoder encoder,
    ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
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
            _logger.LogDebug("Authentication is off");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        var providedApiKey = ParseApiKey();

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            _logger.LogDebug("No Authentication: providedApiKey is blank");
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
            _logger.LogDebug("ApiKey Authentication success");
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
            _logger.LogDebug("Swagger Authentication success");
            return _apiKey;
        }
        var requestPath = Context.Request.Path.Value.ToString();

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

            var crypt = requestPath.GetAPIKeyFromPath();
            if (string.IsNullOrEmpty(crypt))
            {
                _logger.LogDebug("SGLinks: crypt is blank for {requestPath}", requestPath);
            }
           
            return crypt;
        }
        _logger.LogDebug("Authentication start for {requestPath}", requestPath);
        // Try query parameter
        if (Request.Query.TryGetValue(Options.QueryName, out var value))
        {
            _logger.LogDebug("Authentication used query parameter {value}", value.FirstOrDefault());
            return value.FirstOrDefault();
        }

        // No ApiKey query parameter found try headers
        if (Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
        {
            _logger.LogDebug("Authentication used headers {headerName} : {value}", Options.HeaderName,headerValue.FirstOrDefault());
            return headerValue.FirstOrDefault();
        }

        _logger.LogDebug("Authentication used bearer : {value}",  Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", ""));
        return Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
    }
}