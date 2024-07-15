using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;

using NSwag;
using NSwag.Generation.Processors.Security;

using Prometheus;

using StreamMaster.API.Services;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Logging;
using StreamMaster.Infrastructure.Authentication;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.Logger;
using StreamMaster.Infrastructure.Services.Frontend;
using StreamMaster.Infrastructure.Services.QueueService;
using StreamMaster.Streams.Handler;

using StreamMasterAPI.SchemaHelpers;

using System.Reflection;

namespace StreamMaster.API;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        // Register SMLoggerProvider with DI
        services.AddSingleton<ILoggerProvider, SMLoggerProvider>(provider =>
            new SMLoggerProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFilter("StreamMaster.Domain.Logging.CustomLogger", LogLevel.Information);
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            loggingBuilder.AddProvider(new StatsLoggerProvider());

            // GetOrAdd specific filters for StatsLoggerProvider
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

        services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddHealthChecks().AddCheck<StreamHandlerHealthCheck>("stream_handler_health_check");
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

        Assembly assembly = Assembly.Load("StreamMaster.Application");

        services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            })
            .AddApplicationPart(typeof(StaticResourceController).Assembly)
             .AddApplicationPart(assembly)
            .AddControllersAsServices();



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
           ILogger<BackgroundTaskQueue> logger = x.GetRequiredService<ILogger<BackgroundTaskQueue>>();
           ISender sender = x.GetRequiredService<ISender>();
           IDataRefreshService dataRefreshService = x.GetRequiredService<IDataRefreshService>();
           return new BackgroundTaskQueue(queueCapacity, logger, sender, dataRefreshService);
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

            AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder(nameof(AuthenticationType.Forms), "API")
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("SignalR", signalRPolicy);
            options.AddPolicy("SGLinks", sgLinksPolicy);
            options.FallbackPolicy = fallbackPolicy;

        });


        services.AddSignalR().AddMessagePackProtocol();

        services.AddDataProtection().PersistKeysToDbContext<PGSQLRepositoryContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        services.AddAppAuthentication();

        return services;
    }
}