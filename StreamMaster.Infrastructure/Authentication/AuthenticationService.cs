using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Services;

using StreamMaster.Infrastructure.Extensions;

namespace StreamMaster.Infrastructure.Authentication;

public interface IAuthenticationService
{
    Task<User> Login(HttpRequest request, string username, string password);

    Task Logout(HttpContext context);

    void LogUnauthorized(HttpRequest context);
}

public class AuthenticationService(ILogger<AuthenticationService> logger, ISettingsService settingsService) : IAuthenticationService
{
    private async Task<AuthenticationType> GetAuthMethod()
    {
        Setting setting = await settingsService.GetSettingsAsync();
        string AdminPassword = setting.AdminPassword;
        string AdminUserName = setting.AdminUserName;
        AuthenticationType authMethod = AuthenticationType.None;
        if (
            setting.AuthenticationMethod != AuthenticationType.None &&
            !string.IsNullOrEmpty(AdminPassword) && !string.IsNullOrEmpty(AdminUserName)
            )
        {
            authMethod = AuthenticationType.Forms;
        }
        return authMethod;
    }
    public async Task<User> Login(HttpRequest request, string username, string password)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        string AdminPassword = setting.AdminPassword;
        string AdminUserName = setting.AdminUserName;
        AuthenticationType authMethod = await GetAuthMethod();

        if (authMethod == AuthenticationType.None)
        {
            return null;
        }

        if (username == AdminUserName && password == AdminPassword)
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

    public async Task Logout(HttpContext context)
    {
        AuthenticationType authMethod = await GetAuthMethod();
        if (authMethod == AuthenticationType.None)
        {
            return;
        }

        if (context.User != null)
        {
            LogLogout(context.Request, context.User.Identity.Name);
        }
    }

    public void LogUnauthorized(HttpRequest context)
    {
        logger.LogInformation("Auth-Unauthorized ip {0} url '{1}'", context.GetRemoteIP(), context.Path);
    }

    private void LogFailure(HttpRequest context, string username)
    {
        logger.LogWarning("Auth-Failure ip {0} username '{1}'", context.GetRemoteIP(), username);
    }

    private void LogInvalidated(HttpRequest context)
    {
        logger.LogInformation("Auth-Invalidated ip {0}", context.GetRemoteIP());
    }

    private void LogLogout(HttpRequest context, string username)
    {
        logger.LogInformation("Auth-Logout ip {0} username '{1}'", context.GetRemoteIP(), username);
    }

    private void LogSuccess(HttpRequest context, string username)
    {
        logger.LogInformation("Auth-Success ip {0} username '{1}'", context.GetRemoteIP(), username);
    }
}