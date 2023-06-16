using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using NSwag;
using NSwag.Generation.Processors.Security;

using StreamMasterAPI.SchemaHelpers;
using StreamMasterAPI.Services;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

using StreamMasterDomain.Configuration;
using StreamMasterDomain.EnvironmentInfo;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Authentication;
using StreamMasterInfrastructure.Persistence;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.QueueService;

namespace StreamMasterAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        _ = services.AddLogging();
        _ = services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddResponseCompression(options => options.EnableForHttps = true);
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddCors(options =>
        {
            options.AddPolicy("DevPolicy",
                builder =>
            builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                );

            options.AddPolicy(VersionedApiControllerAttribute.API_CORS_POLICY,
                builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            options.AddPolicy("AllowGet",
                builder =>
                builder.AllowAnyOrigin()
                .WithMethods("GET", "OPTIONS")
                .AllowAnyHeader());
        });

        _ = services.AddSession();

        // services.AddTransient<IMapHttpRequestsToDisk, HtmlMapperBase>();

        // services
        //.AddControllers(options =>
        //{
        //    options.ReturnHttpNotAcceptable = true;
        //})
        //.AddApplicationPart(typeof(StaticResourceController).Assembly)
        //.AddControllersAsServices();

        _ = services.AddDatabaseDeveloperPageExceptionFilter();

        _ = services.AddSingleton<ICurrentUserService, CurrentUserService>();
        _ = services.AddSingleton<IAppFolderInfo, AppFolderInfo>();
        _ = services.AddSingleton<IConfigFileProvider, ConfigFileProvider>();

        _ = services.AddScoped<IAuthenticationService, AuthenticationService>();

        _ = services.AddHttpContextAccessor();

        _ = services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

        _ = services.AddFluentValidationAutoValidation();

        _ = services.AddHttpClient();

        _ = services.AddControllersWithViews();

        _ = services.AddRazorPages();

        _ = services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        _ = services.AddHostedService<QueuedHostedService>();

        _ = services.AddSingleton<IBackgroundTaskQueue>(x =>
       {
           int queueCapacity = 100;
           return new BackgroundTaskQueue(queueCapacity, x.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>(), x.GetRequiredService<ILogger<BackgroundTaskQueue>>(), x.GetRequiredService<ISender>());
       });

        _ = services.AddOpenApiDocument(configure =>
        {
            configure.Title = "StreamMaster API";
            configure.SchemaProcessors.Add(new InheritanceSchemaProcessor());
            
            configure.AddSecurity("apikey", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "x-api-key",
                In = OpenApiSecurityApiKeyLocation.Header
            });

            configure.OperationProcessors.Add(
       new AspNetCoreOperationSecurityScopeProcessor("apikey"));
        });

        _ = services.AddHostedService<PostStartup>();
        _ = services.AddSingleton<PostStartup>();

        _ = services.AddSignalR();//.AddMessagePackProtocol();
        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SignalR", policy =>
            {
                policy.AuthenticationSchemes.Add("SignalR");
                policy.RequireAuthenticatedUser();
            });

            // Require auth on everything except those marked [AllowAnonymous]
            options.FallbackPolicy = new AuthorizationPolicyBuilder("API")
            .RequireAuthenticatedUser()
            .Build();
        });

        services.AddAppAuthentication();
        return services;
    }
}
