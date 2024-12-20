using System.Reflection;

using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

using NSwag;
using NSwag.Generation.Processors.Security;

using StreamMaster.API.SchemaHelpers;
using StreamMaster.API.Services;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Logging;
using StreamMaster.Infrastructure.Authentication;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.Infrastructure.Services.Frontend;
using StreamMaster.Infrastructure.Services.QueueService;

namespace StreamMaster.API;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services, WebApplicationBuilder builder, bool DBDebug)
    {
        // Register SMLoggerProvider with DI
        //services.AddSingleton<ILoggerProvider, SMLoggerProvider>(provider =>
        //    new SMLoggerProvider(provider.GetRequiredService<IFileLoggingServiceFactory>()));

        //_ = services.AddSingleton(provider =>
        //{
        //    IFileLoggingServiceFactory factory = provider.GetRequiredService<IFileLoggingServiceFactory>();
        //    return factory.Create("SMLogger");
        //});
        //services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddLogging(loggingBuilder =>
        {
            string test = DbLoggerCategory.Database.Command.Name;

            loggingBuilder.AddFilter("StreamMaster.Domain.Logging.CustomLogger", LogLevel.Information);
            if (DBDebug)
            {
                loggingBuilder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
            }
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            //loggingBuilder.AddProvider(new SMLoggerProvider(new FileLoggingServiceFactory(builder.Configuration)));
            //loggingBuilder.AddProvider(new StatsLoggerProvider());

            // GetOrAdd specific filters for StatsLoggerProvider
            //loggingBuilder.AddFilter<StatsLoggerProvider>((category, _) =>
            //{
            //    // List of classes to use with CustomLogger
            //    List<string> classesToLog = ["BroadcastService"];
            //    return category != null && classesToLog.Any(c => category.Contains(c, StringComparison.OrdinalIgnoreCase));
            //});

            ServiceProvider serviceProvider = loggingBuilder.Services.BuildServiceProvider();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            GlobalLoggerProvider.Configure(loggerFactory);
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

        //services.AddDatabaseDeveloperPageExceptionFilter();

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
           const int queueCapacity = 100;
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
        //services.AddSingleton<PostStartup>();

        services.AddAppAuthenticationAndAuthorization();

        services.AddSignalR().AddMessagePackProtocol();

        services.AddDataProtection().PersistKeysToDbContext<PGSQLRepositoryContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, UiAuthorizationPolicyProvider>();

        return services;
    }
}