using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;

using NSwag;
using NSwag.Generation.Processors.Security;

using StreamMasterAPI.SchemaHelpers;
using StreamMasterAPI.Services;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

using StreamMasterDomain.EnvironmentInfo;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Authentication;
using StreamMasterInfrastructure.EF;
using StreamMasterInfrastructure.Logging;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.Frontend;
using StreamMasterInfrastructure.Services.QueueService;

namespace StreamMasterAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
            logging.AddProvider(new SMLoggerProvider());
        });

        services.AddMemoryCache();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddResponseCompression(options => options.EnableForHttps = true);

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

        _ = services.AddDatabaseDeveloperPageExceptionFilter();

        services
            .AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddApplicationPart(typeof(StaticResourceController).Assembly)
            .AddControllersAsServices();

        _ = services.AddSingleton<IAppFolderInfo, AppFolderInfo>();

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

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SignalR", policy =>
            {
                policy.AuthenticationSchemes.Add("SignalR");
                policy.RequireAuthenticatedUser();
            });

            options.AddPolicy("SGLinks", policy =>
            {
                policy.AuthenticationSchemes.Add("SGLinks");
                policy.RequireAuthenticatedUser();
            });

            // Require auth on everything except those marked [AllowAnonymous]
            options.FallbackPolicy = new AuthorizationPolicyBuilder("API")
            .RequireAuthenticatedUser()
            .Build();
        });

        _ = services.AddSignalR();//.AddMessagePackProtocol();

        services.AddDataProtection()
             .PersistKeysToDbContext<AppDbContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        services.AddAppAuthentication();
        return services;
    }
}
