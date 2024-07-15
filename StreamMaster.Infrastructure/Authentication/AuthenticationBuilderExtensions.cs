using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, string name, Action<ApiKeyAuthenticationOptions> options)
    {
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(name, options);
    }

    public static AuthenticationBuilder AddAppAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "API";
            options.DefaultChallengeScheme = "API";
        })
            .AddNone(nameof(AuthenticationType.None))
            //.AddBasic(AuthenticationType.Basic.ToString())            
            .AddCookie(nameof(AuthenticationType.Forms), options =>
            {
                options.Cookie.Name = "StreamMasterAuth";
                options.AccessDeniedPath = "/login?loginFailed=true";
                options.LoginPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            })
            .AddApiKey("API", options =>
            {
                options.HeaderName = "X-Api-Key";
                options.QueryName = "apikey";
            })
             .AddApiKey("SGLinks", options =>
             {
                 options.HeaderName = "SGLinks";
                 options.QueryName = "SGLinks";
             })
            .AddApiKey("SignalR", options =>
            {
                options.HeaderName = "X-Api-Key";
                options.QueryName = "access_token";
            });
    }

    public static AuthenticationBuilder AddBasic(this AuthenticationBuilder authenticationBuilder, string name)
    {
        return authenticationBuilder.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(name, options => { });
    }

    public static AuthenticationBuilder AddNone(this AuthenticationBuilder authenticationBuilder, string name)
    {
        return authenticationBuilder.AddScheme<AuthenticationSchemeOptions, NoAuthenticationHandler>(name, options => { });
    }
}
