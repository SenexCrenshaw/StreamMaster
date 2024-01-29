using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;

using NSwag;
using NSwag.Generation.Processors.Security;

using Prometheus;

using StreamMaster.API.Services;
using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.EnvironmentInfo;
using StreamMaster.Domain.Logging;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure;
using StreamMaster.Infrastructure.Authentication;
using StreamMaster.Infrastructure.EF;
using StreamMaster.Infrastructure.Logger;
using StreamMaster.Infrastructure.Services.Frontend;
using StreamMaster.Infrastructure.Services.QueueService;

using StreamMasterAPI.SchemaHelpers;

namespace StreamMaster.API;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        // Register SMLoggerProvider with DI
        services.AddSingleton<ILoggerProvider, SMLoggerProvider>(provider =>
            new SMLoggerProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        //services.AddSingleton<ILoggerProvider, FileLoggerDebugProvider>(provider =>
        //    new FileLoggerDebugProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        //services.AddLogging(logging =>
        //{
        //    logging.AddFilter("StreamMaster.Domain.Logging.CustomLogger", LogLevel.Information);
        //    logging.AddProvider(new StatsLoggerProvider());
        //    logging.AddConsole();
        //    logging.AddDebug();

        //    ServiceProvider serviceProvider = logging.Services.BuildServiceProvider();
        //    ILoggerProvider loggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();

        //    logging.AddProvider(loggerProvider);

        //    logging.AddFilter<StatsLoggerProvider>((category, logLevel) =>
        //    {
        //        // List of classes to use with CustomLogger
        //        List<string> classesToLog = ["BroadcastService"];
        //        return category is not null && category.Contains("BroadcastService", StringComparison.OrdinalIgnoreCase);
        //    });

        //});

        // Add logging configuration
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFilter("StreamMaster.Domain.Logging.CustomLogger", LogLevel.Information);
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddProvider(new StatsLoggerProvider());

            // Add specific filters for StatsLoggerProvider
            loggingBuilder.AddFilter<StatsLoggerProvider>((category, logLevel) =>
            {
                // List of classes to use with CustomLogger
                List<string> classesToLog = ["BroadcastService"];
                return category != null && classesToLog.Any(c => category.Contains(c, StringComparison.OrdinalIgnoreCase));
            });

            // Consider not manually adding ILoggerProvider here, it's already added above
            ServiceProvider serviceProvider = loggingBuilder.Services.BuildServiceProvider();
            // ILoggerProvider loggerProvider = serviceProvider.GetRequiredService<ILoggerProvider>();
            // loggingBuilder.AddProvider(loggerProvider);
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            GlobalLoggerProvider.Configure(loggerFactory);
        });

        services.AddHttpLogging(o => o = new HttpLoggingOptions());
        services.UseHttpClientMetrics();

        //services.AddTransient(typeof(ILogger<>), typeof(CustomLogger<>));

        //ILoggerFactory loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        //GlobalLoggerProvider.Configure(loggerFactory);

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

        services.AddSession();

        services.AddDatabaseDeveloperPageExceptionFilter();

        services
            .AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            })
            .AddApplicationPart(typeof(StaticResourceController).Assembly)
            .AddControllersAsServices();

        services.AddSingleton<IAppFolderInfo, AppFolderInfo>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddHttpContextAccessor();

        services.AddFluentValidationAutoValidation();

        services.AddHttpClient();

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddHostedService<QueuedHostedService>();

        services.AddSingleton<IBackgroundTaskQueue>(x =>
       {
           int queueCapacity = 100;
           return new BackgroundTaskQueue(queueCapacity, x.GetRequiredService<IHubContext<StreamMasterHub, IStreamMasterHub>>(), x.GetRequiredService<ILogger<BackgroundTaskQueue>>(), x.GetRequiredService<ISender>());
       });

        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "StreamMaster API";
            configure.SchemaSettings.SchemaProcessors.Add(new InheritanceSchemaProcessor());

            configure.AddSecurity("apikey", [], new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "x-api-key",
                In = OpenApiSecurityApiKeyLocation.Header
            });

            configure.OperationProcessors.Add(
       new AspNetCoreOperationSecurityScopeProcessor("apikey"));
        });

        services.AddHostedService<PostStartup>();
        services.AddSingleton<PostStartup>();

        //services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("SignalR", policy =>
        //    {
        //        policy.AuthenticationSchemes.Add("SignalR");
        //        policy.RequireAuthenticatedUser();
        //    });

        //    options.AddPolicy("SGLinks", policy =>
        //    {
        //        policy.AuthenticationSchemes.Add("SGLinks");
        //        policy.RequireAuthenticatedUser();
        //    });

        //    // Require auth on everything except those marked [AllowAnonymous]
        //    options.FallbackPolicy = new AuthorizationPolicyBuilder(AuthenticationType.Forms.ToString(), "API")
        //    .RequireAuthenticatedUser()
        //    .Build();
        //});

        services.AddAuthorization(options =>
        {

            AuthorizationPolicy signalRPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SignalR")
                .RequireAuthenticatedUser()
                .Build();

            AuthorizationPolicy sgLinksPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SGLinks")
                .RequireAuthenticatedUser()
                .Build();

            AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder(AuthenticationType.Forms.ToString(), "API")
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("SignalR", signalRPolicy);
            options.AddPolicy("SGLinks", sgLinksPolicy);
            options.FallbackPolicy = fallbackPolicy;

            //// Define the "SignalR" policy
            //options.AddPolicy("SignalR", policyBuilder =>
            //    policyBuilder
            //        .AddAuthenticationSchemes("SignalR")
            //        .RequireAuthenticatedUser()
            //);

            //// Define the "SGLinks" policy
            //options.AddPolicy("SGLinks", policyBuilder =>
            //    policyBuilder
            //        .AddAuthenticationSchemes("SGLinks")
            //        .RequireAuthenticatedUser()
            //);

            // Define the FallbackPolicy

        });


        services.AddSignalR();//.AddMessagePackProtocol();

        services.AddDataProtection()
             .PersistKeysToDbContext<RepositoryContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        services.AddAppAuthentication();

        return services;
    }
}