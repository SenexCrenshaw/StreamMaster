using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static void AddAppAuthenticationAndAuthorization(this IServiceCollection services)
    {
        // Authentication setup
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "SGLinks";// nameof(AuthenticationType.Forms); 
            options.DefaultChallengeScheme = "SGLinks";//nameof(AuthenticationType.Forms);
        })
        .AddNone(nameof(AuthenticationType.None))
        .AddCookie(nameof(AuthenticationType.Forms), options =>
        {
            options.Cookie.Name = "StreamMasterAuth";
            options.AccessDeniedPath = "/login?loginFailed=true";
            options.LoginPath = "/login";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
        }).AddApiKey("SGLinks", options =>
        {
            options.HeaderName = "SGLinks";
            options.QueryName = "SGLinks";
        }).AddApiKey("SignalR", options =>
        {
            options.HeaderName = "SignalR";
            options.QueryName = "SignalR";
        });

        // Authorization setup
        services.AddAuthorization(options =>
        {
            // SignalR policy using the default cookie authentication scheme
            AuthorizationPolicy signalRPolicy = new AuthorizationPolicyBuilder()
                 .AddAuthenticationSchemes("SignalR")
                .RequireAuthenticatedUser()
                .Build();

            // SGLinks policy using SGLinks-specific scheme (as an example)
            AuthorizationPolicy sgLinksPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SGLinks")
                .RequireAuthenticatedUser()
                .Build();

            // Fallback policy for default authentication using Cookies
            AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SGLinks")//.AddAuthenticationSchemes(nameof(AuthenticationType.Forms))
                .RequireAuthenticatedUser()
                .Build();

            // Adding the policies
            options.AddPolicy("SignalR", signalRPolicy);
            options.AddPolicy("SGLinks", sgLinksPolicy);
            options.FallbackPolicy = fallbackPolicy;
        });
    }

    // Custom methods to add specific authentication handlers
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, string name, Action<ApiKeyAuthenticationOptions> options)
    {
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(name, options);
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
