using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using StreamMasterAPI.SchemaHelpers;
using StreamMasterAPI.Services;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

using StreamMasterInfrastructure.Persistence;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.QueueService;

namespace StreamMasterAPI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        _ = services.AddSession();

        _ = services.AddSignalR();//.AddMessagePackProtocol();

        _ = services.AddDatabaseDeveloperPageExceptionFilter();

        _ = services.AddSingleton<ICurrentUserService, CurrentUserService>();

        _ = services.AddHttpContextAccessor();

        _ = services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

        _ = services.AddLogging();

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
       });

        _ = services.AddHostedService<PostStartup>();
        _ = services.AddSingleton<PostStartup>();

        return services;
    }
}
