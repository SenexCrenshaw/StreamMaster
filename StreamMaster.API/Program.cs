using MediatR;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;

using Prometheus;

using Reinforced.Typings.Attributes;

using StreamMaster.API;
using StreamMaster.Application;
using StreamMaster.Application.General.Commands;
using StreamMaster.Application.Hubs;

using StreamMaster.Domain.Helpers;

using StreamMaster.Infrastructure;
using StreamMaster.Infrastructure.EF;
using StreamMaster.Infrastructure.EF.PGSQL;

using StreamMaster.Infrastructure.Middleware;
using StreamMaster.SchedulesDirect.Services;
using StreamMaster.Streams;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

[assembly: TsGlobal(CamelCaseForProperties = false, CamelCaseForMethods = false, UseModules = true, DiscardNamespacesWhenUsingModules = true, AutoOptionalProperties = true, WriteWarningComment = false, ReorderMembers = true)]
//ProcessHelper.KillProcessByName("ffmpeg");

//DirectoryHelper.RenameDirectory(Path.Combine(BuildInfo.AppDataFolder, "hls"), BuildInfo.HLSOutputFolder);
//DirectoryHelper.RenameDirectory(Path.Combine(BuildInfo.AppDataFolder, "settings"), BuildInfo.SettingsFolder);
//DirectoryHelper.RenameDirectory(Path.Combine(BuildInfo.AppDataFolder, "backups"), BuildInfo.BackupFolder);

DirectoryHelper.CreateApplicationDirectories();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


static void Log(string format, params object[] args)
{
    string message = string.Format(format, args);
    Console.WriteLine(message);
    Debug.WriteLine(message);
}

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.AllowSynchronousIO = true;
    serverOptions.Limits.MaxRequestBodySize = null;
});


var settingsFiles = BuildInfo.GetSettingFiles();

builder.Configuration.SetBasePath(BuildInfo.StartUpPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);


if (Directory.Exists(BuildInfo.SettingsFolder))
{
    builder.Configuration.SetBasePath(BuildInfo.AppDataFolder);
}


var videoProfileSetting = SettingsHelper.GetSetting<VideoOutputProfiles>(BuildInfo.VideoProfileSettingsFile);
if (videoProfileSetting == default(VideoOutputProfiles))
{
    SettingsHelper.UpdateSetting(SettingFiles.DefaultVideoProfileSetting);
}

var fileProfileSetting = SettingsHelper.GetSetting<OutputProfiles>(BuildInfo.OutputProfileSettingsFile);
if (fileProfileSetting == default(OutputProfiles))
{
    SettingsHelper.UpdateSetting(SettingFiles.DefaultOutputProfileSetting);
}


var hlsSetting = SettingsHelper.GetSetting<HLSSettings>(BuildInfo.HLSSettingsFile);
if (hlsSetting == default(HLSSettings))
{
    SettingsHelper.UpdateSetting(new HLSSettings());
}

//var mainSetting = SettingsHelper.GetSetting<OldSetting>(BuildInfo.SettingsFile);
//if (mainSetting != default(OldSetting))
//{
//    if (mainSetting.SDSettings != default(SDSettings))
//    {
//        SettingsHelper.UpdateSetting(mainSetting.SDSettings);
//        var toWrite = mainSetting.ConvertToSetting();
//        SettingsHelper.UpdateSetting(toWrite);
//    }
//}


var mainSetting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);
if (mainSetting == default(Setting))
{
    SettingsHelper.UpdateSetting(new Setting());
}

var sdSettings = SettingsHelper.GetSetting<SDSettings>(BuildInfo.SDSettingsFile);
if (sdSettings == default(SDSettings))
{
    SettingsHelper.UpdateSetting(new SDSettings());
}


foreach (var file in settingsFiles)
{
    if (File.Exists(file))
    {
        Log($"Using settings file {file}");
        builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);
    }
}

builder.Services.Configure<Setting>(builder.Configuration);
builder.Services.Configure<SDSettings>(builder.Configuration);
builder.Services.Configure<HLSSettings>(builder.Configuration);
builder.Services.Configure<VideoOutputProfiles>(builder.Configuration);
builder.Services.Configure<OutputProfiles>(builder.Configuration);



bool enableSsl = false;

string? sslCertPath = builder.Configuration["SSLCertPath"];
string? sslCertPassword = builder.Configuration["sslCertPassword"];

if (!bool.TryParse(builder.Configuration["EnableSSL"], out enableSsl))
{
}

List<string> urls = ["http://0.0.0.0:7095"];

if (enableSsl && !string.IsNullOrEmpty(sslCertPath))
{
    urls.Add("https://0.0.0.0:7096");
}

builder.WebHost.UseUrls(urls.ToArray());

if (!string.IsNullOrEmpty(sslCertPath))
{
    if (string.IsNullOrEmpty(sslCertPassword))
    {
        sslCertPassword = "";
    }

    _ = builder.WebHost.ConfigureKestrel(options =>
    options.ConfigureHttpsDefaults(configureOptions =>
       configureOptions.ServerCertificate = ValidateSslCertificate(Path.Combine(BuildInfo.AppDataFolder, sslCertPath), sslCertPassword)
    ));
}

// GetOrAdd services to the container.
builder.Services.AddSchedulesDirectAPIServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureEFPGSQLServices();
builder.Services.AddInfrastructureEFServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddInfrastructureServicesEx();
builder.Services.AddStreamsServices();
builder.Services.AddWebUIServices(builder);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
}).AddXmlSerializerFormatters()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
;

WebApplication app = builder.Build();
app.UseForwardedHeaders();

var lifetime = app.Services.GetService<IHostApplicationLifetime>();
if (lifetime != null)
{
    lifetime.ApplicationStopping.Register(OnShutdown);
}

void OnShutdown()
{
    var sender = app.Services.GetRequiredService<ISender>();
    sender.Send(new SetIsSystemReadyRequest(false)).Wait();
    ProcessHelper.KillProcessByName("ffmpeg");
    SqliteConnection.ClearAllPools();
    PGSQLRepositoryContext repositoryContext = app.Services.GetRequiredService<PGSQLRepositoryContext>();
    repositoryContext.Dispose();
    IImageDownloadService imageDownloadService = app.Services.GetRequiredService<IImageDownloadService>();
    imageDownloadService.StopAsync(CancellationToken.None).Wait();

    DirectoryHelper.EmptyDirectory(BuildInfo.HLSOutputFolder);

    FileUtil.Backup().Wait();
}

app.UseOpenApi();
app.UseSwaggerUi();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
    _ = app.UseMigrationsEndPoint();

}
else
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

app.UseHttpLogging();
app.UseMigrationsEndPoint();
app.UseSession();

//app.UseHangfireDashboard();

using (IServiceScope scope = app.Services.CreateScope())
{
    LogDbContextInitialiser logInitialiser = scope.ServiceProvider.GetRequiredService<LogDbContextInitialiser>();
    await logInitialiser.InitialiseAsync().ConfigureAwait(false);
    if (app.Environment.IsDevelopment())
    {
        logInitialiser.TrySeed();

    }

    RepositoryContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<RepositoryContextInitializer>();
    
    await initialiser.InitializeAsync(mainSetting!).ConfigureAwait(false);
    if (app.Environment.IsDevelopment())
    {
        initialiser.TrySeed();
    }

    initialiser.MigrateData();

    IImageDownloadService imageDownloadService = scope.ServiceProvider.GetRequiredService<IImageDownloadService>();
    imageDownloadService.Start();
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();
app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    _ = app.UseCors("DevPolicy");
}
else
{
    _ = app.UseCors();
}
//_ = app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CacheHeaderMiddleware>();
//if (app.Environment.IsDevelopment())
//{
//    //RecurringJob.AddOrUpdate("Hello World", () => Console.WriteLine("hello world"), Cron.Minutely);    
//}
//else
//{
//    _ = app.UseResponseCompression();
//}

app.MapDefaultControllerRoute();

app.Map("/swagger", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.MapGet("/routes", async context =>
{
    EndpointDataSource endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();

    foreach (Endpoint endpoint in endpointDataSource.Endpoints)
    {
        string routePattern = GetRoutePattern(endpoint);

        await context.Response.WriteAsync($"Route: {routePattern}\n");
    }
});

app.MapHub<StreamMasterHub>("/streammasterhub");//.RequireAuthorization(AuthenticationType.Forms.ToString());
app.MapMetrics();

app.Run();


static string GetRoutePattern(Endpoint endpoint)
{
    RouteEndpoint? routeEndpoint = endpoint as RouteEndpoint;

    return routeEndpoint is not null && routeEndpoint.RoutePattern is not null && routeEndpoint.RoutePattern.RawText is not null
        ? routeEndpoint.RoutePattern.RawText
        : "<unknown>";
}


static X509Certificate2 ValidateSslCertificate(string cert, string password)
{
    X509Certificate2 certificate;

    try
    {
        certificate = new X509Certificate2(cert, password, X509KeyStorageFlags.DefaultKeySet);
    }
    catch (CryptographicException ex)
    {
        if (ex.HResult is 0x2 or 0x2006D080)
        {
            throw new Exception($"The SSL certificate file {cert} does not exist: {ex.Message}");
        }

        throw;
    }

    return certificate;
}
