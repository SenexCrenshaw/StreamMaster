using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Repository;

using StreamMasterInfrastructure.Extensions;

namespace StreamMasterInfrastructure.Authentication;

public interface IAuthenticationService
{
    User Login(HttpRequest request, string username, string password);

    void Logout(HttpContext context);

    void LogUnauthorized(HttpRequest context);
}

public class AuthenticationService : IAuthenticationService
{
    private static string AdminPassword;
    private static string AdminUserName;
    private static AuthenticationType AUTH_METHOD;
    private readonly ILogger<AuthenticationService> _logger;
    protected Setting _setting = FileUtil.GetSetting();

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;
        AdminPassword = _setting.AdminPassword;
        AdminUserName = _setting.AdminUserName;
        var authMethod = AuthenticationType.None;
        if (
            _setting.AuthenticationMethod != AuthenticationType.None &&
            (!string.IsNullOrEmpty(AdminPassword) && !string.IsNullOrEmpty(AdminUserName))
            )
        {
            authMethod = AuthenticationType.Forms;
        }

        AUTH_METHOD = authMethod;
    }

    public User Login(HttpRequest request, string username, string password)
    {
        if (AUTH_METHOD == AuthenticationType.None)
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

    public void Logout(HttpContext context)
    {
        if (AUTH_METHOD == AuthenticationType.None)
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
        _logger.LogInformation("Auth-Unauthorized ip {0} url '{1}'", context.GetRemoteIP(), context.Path);
    }

    private void LogFailure(HttpRequest context, string username)
    {
        _logger.LogWarning("Auth-Failure ip {0} username '{1}'", context.GetRemoteIP(), username);
    }

    private void LogInvalidated(HttpRequest context)
    {
        _logger.LogInformation("Auth-Invalidated ip {0}", context.GetRemoteIP());
    }

    private void LogLogout(HttpRequest context, string username)
    {
        _logger.LogInformation("Auth-Logout ip {0} username '{1}'", context.GetRemoteIP(), username);
    }

    private void LogSuccess(HttpRequest context, string username)
    {
        _logger.LogInformation("Auth-Success ip {0} username '{1}'", context.GetRemoteIP(), username);
    }
}