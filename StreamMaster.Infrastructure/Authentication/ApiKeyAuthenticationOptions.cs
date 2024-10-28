﻿using Microsoft.AspNetCore.Authentication;
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
    public static string Scheme => DefaultScheme;
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    IOptionsMonitor<Setting> settings,
    IDataRefreshService dataRefreshService,
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
        bool needsAuth = string.IsNullOrEmpty(settings.CurrentValue.AuthenticationMethod) || !settings.CurrentValue.AuthenticationMethod.Equals("none", StringComparison.CurrentCultureIgnoreCase);

        if (!needsAuth)
        {
            List<Claim> claims =
        [
            new Claim("ApiKey", "true")
        ];

            ClaimsIdentity identity = new(claims, Options.AuthenticationType);
            ClaimsPrincipal principal = new(identity);
            AuthenticationTicket ticket = new(principal, ApiKeyAuthenticationOptions.Scheme);
            _logger.LogDebug("Authentication is off");
            return AuthenticateResult.Success(ticket);
        }

        // Try to authenticate with an API key first
        string? providedApiKey = await ParseApiKeyAsync(settings.CurrentValue.ServerKey);
        if (!string.IsNullOrWhiteSpace(providedApiKey))
        {
            if (settings.CurrentValue.ServerKey == providedApiKey)
            {
                List<Claim> claims =
            [
                new Claim("ApiKey", "true")
            ];

                ClaimsIdentity identity = new(claims, Options.AuthenticationType);
                ClaimsPrincipal principal = new(identity);
                AuthenticationTicket ticket = new(principal, ApiKeyAuthenticationOptions.Scheme);
                _logger.LogDebug("ApiKey Authentication success");
                return AuthenticateResult.Success(ticket);
            }
        }

        // If API key authentication fails, try Forms authentication (cookie-based)
        AuthenticateResult cookieAuthResult = await Context.AuthenticateAsync(nameof(AuthenticationType.Forms));
        if (cookieAuthResult.Succeeded && cookieAuthResult.Principal != null)
        {
            _logger.LogDebug("Authentication success using Forms cookie for {requestPath}", Context.Request.Path);
            return AuthenticateResult.Success(cookieAuthResult.Ticket);
        }

        // Both API key and Forms authentication failed
        _logger.LogDebug("No valid API key or cookie found for authentication");
        //await dataRefreshService.AuthLogOut();
        return AuthenticateResult.Fail("No valid API key or cookie found for authentication");
    }

    //protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    //{
    //    // If authentication fails, redirect to the login page
    //    if (Context.Request.Path.StartsWithSegments("/streammasterhub"))
    //    {
    //        _logger.LogDebug("SignalR authentication failed. Redirecting to login.");
    //        Response.Redirect("/login");
    //    }
    //    else
    //    {
    //        Response.StatusCode = 401; // Set unauthorized status for non-SignalR requests
    //    }

    //    return Task.CompletedTask;
    //}

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // If authentication fails, redirect to the login page
        if (!Context.User.Identity.IsAuthenticated)
        {
            _logger.LogDebug("Authentication failed. Redirecting to login page for {requestPath}", Context.Request.Path);

            dataRefreshService.AuthLogOut();
            // Redirecting to login path
            Response.Redirect("/login");
        }
        else
        {
            // If already authenticated, but insufficient permissions (403 forbidden)
            Response.StatusCode = 403;
        }

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
            return Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
        }

        return null;
    }
}