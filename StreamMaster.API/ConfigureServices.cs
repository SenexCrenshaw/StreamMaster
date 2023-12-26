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

using Prometheus;

using StreamMaster.Domain.Enums;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Services;

using StreamMasterAPI.SchemaHelpers;
using StreamMasterAPI.Services;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Common.Logging;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.Services;

using StreamMaster.Infrastructure;
using StreamMaster.Infrastructure.Authentication;
using StreamMaster.Infrastructure.Logger;
using StreamMaster.Infrastructure.Services;
using StreamMaster.Infrastructure.Services.Frontend;
using StreamMaster.Infrastructure.Services.QueueService;

using StreamMaster.Infrastructure.EF;

namespace StreamMasterAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        // Register SMLoggerProvider with DI
        services.AddSingleton<ILoggerProvider, SMLoggerProvider>(provider =>
            new SMLoggerProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        //services.AddSingleton<ILoggerProvider, FileLoggerDebugProvider>(provider =>
        //    new FileLoggerDebugProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        services.AddLogging(logging =>
        {
            logging.AddFilter("StreamMasterDomain.Logging.CustomLogger", LogLevel.Information);
            logging.AddProvider(new StatsLoggerProvider());
            logging.AddConsole();
            logging.AddDebug();

            ServiceProvider serviceProvider = logging.Services.BuildServiceProvider();
            ILoggerProvider loggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();

            logging.AddProvider(loggerProvider);

            logging.AddFilter<StatsLoggerProvider>((category, logLevel) =>
            {
                // List of classes to use with CustomLogger
                List<string> classesToLog = ["BroadcastService"];
                return category is not null && category.Contains("BroadcastService", StringComparison.OrdinalIgnoreCase);
            });

        });

        services.UseHttpClientMetrics();

        services.AddTransient(typeof(ILogger<>), typeof(CustomLogger<>));

        ILoggerFactory loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        GlobalLoggerProvider.Configure(loggerFactory);

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
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            options.AddPolicy("AllowGet",
                builder =>
                builder
                .AllowAnyOrigin()
                //.WithMethods("GET", "OPTIONS")
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        _ = services.AddSession();

        _ = services.AddDatabaseDeveloperPageExceptionFilter();

        services
            .AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            })
            .AddApplicationPart(typeof(StaticResourceController).Assembly)
            .AddControllersAsServices();

        _ = services.AddSingleton<IAppFolderInfo, AppFolderInfo>();

        _ = services.AddScoped<IAuthenticationService, AuthenticationService>();

        _ = services.AddHttpContextAccessor();

        _ = services.AddFluentValidationAutoValidation();

        services.AddHttpClient();

        services.AddControllersWithViews();
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
            options.FallbackPolicy = new AuthorizationPolicyBuilder(AuthenticationType.Forms.ToString(), "API")
            .RequireAuthenticatedUser()
            .Build();
        });

        _ = services.AddSignalR();//.AddMessagePackProtocol();

        services.AddDataProtection()
             .PersistKeysToDbContext<RepositoryContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        services.AddAppAuthentication();

        return services;
    }
}