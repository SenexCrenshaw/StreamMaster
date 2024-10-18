using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;
using StreamMaster.Infrastructure.Extensions;

namespace StreamMaster.Infrastructure.Authentication;

public interface IAuthenticationService
{
    User? Login(HttpRequest request, string username, string password);
    void Logout(HttpContext context);
    void LogUnauthorized(HttpRequest context);
}

public class AuthenticationService(ILogger<AuthenticationService> logger, IOptionsMonitor<Setting> settings) : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger = logger;

    private AuthenticationType GetAuthMethod()
    {
        string adminPassword = settings.CurrentValue.AdminPassword;
        string adminUserName = settings.CurrentValue.AdminUserName;
        AuthenticationType authMethod = AuthenticationType.None;

        if (settings.CurrentValue.AuthenticationMethod != "None" &&
            !string.IsNullOrEmpty(adminPassword) &&
            !string.IsNullOrEmpty(adminUserName))
        {
            authMethod = AuthenticationType.Forms;
        }

        return authMethod;
    }

    public User? Login(HttpRequest request, string username, string password)
    {
        string adminPassword = settings.CurrentValue.AdminPassword;
        string adminUserName = settings.CurrentValue.AdminUserName;
        AuthenticationType authMethod = GetAuthMethod();

        if (authMethod == AuthenticationType.None)
        {
            return null;
        }

        if (username == adminUserName && password == adminPassword)
        {
            LogSuccess(request, username);

            return new User
            {
                Username = username,
                Identifier = Guid.NewGuid(),
                Password = password
            };
        }

        LogFailure(request, username);
        return null;
    }

    public void Logout(HttpContext context)
    {
        AuthenticationType authMethod = GetAuthMethod();
        if (authMethod == AuthenticationType.None)
        {
            return;
        }

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            LogLogout(context.Request, context.User.Identity.Name ?? "Unknown");
        }
    }

    public void LogUnauthorized(HttpRequest context)
    {
        _logger.LogInformation("Auth-Unauthorized ip {RemoteIP} url '{Path}'", context.GetRemoteIP(), context.Path);
    }

    private void LogFailure(HttpRequest context, string username)
    {
        _logger.LogWarning("Auth-Failure ip {RemoteIP} username '{Username}'", context.GetRemoteIP(), username);
    }

    private void LogInvalidated(HttpRequest context)
    {
        _logger.LogInformation("Auth-Invalidated ip {RemoteIP}", context.GetRemoteIP());
    }

    private void LogLogout(HttpRequest context, string? username)
    {
        _logger.LogInformation("Auth-Logout ip {RemoteIP} username '{Username}'", context.GetRemoteIP(), username ?? "Unknown");
    }

    private void LogSuccess(HttpRequest context, string username)
    {
        _logger.LogInformation("Auth-Success ip {RemoteIP} username '{Username}'", context.GetRemoteIP(), username);
    }
}
