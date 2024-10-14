using MediatR;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;

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
using StreamMaster.PlayList;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;

[assembly: TsGlobal(CamelCaseForProperties = false, CamelCaseForMethods = false, UseModules = true, DiscardNamespacesWhenUsingModules = true, AutoOptionalProperties = true, WriteWarningComment = false, ReorderMembers = true)]

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

builder.WebHost.ConfigureKestrel((_, serverOptions) =>
{
    serverOptions.AllowSynchronousIO = true;
    serverOptions.Limits.MaxRequestBodySize = null;
});

var settingsFiles = BuildInfo.GetSettingFiles();

// Set base configuration path
var configPath = Directory.Exists(BuildInfo.SettingsFolder) ? BuildInfo.AppDataFolder : BuildInfo.StartUpPath;
builder.Configuration.SetBasePath(configPath).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Load and validate settings
LoadAndSetSettings<CommandProfileDict, CommandProfile>(BuildInfo.CommandProfileSettingsFile, SettingFiles.DefaultCommandProfileSetting);
LoadAndSetSettings<OutputProfileDict, OutputProfile>(BuildInfo.OutputProfileSettingsFile, SettingFiles.DefaultOutputProfileSetting);
//LoadAndValidateSettings<HLSSettings>(BuildInfo.HLSSettingsFile, new HLSSettings());
LoadAndValidateSettings<Setting>(BuildInfo.SettingsFile, new Setting());
LoadAndValidateSettings<SDSettings>(BuildInfo.SDSettingsFile, new SDSettings());

// Add additional settings files if they exist
foreach (var file in settingsFiles)
{
    if (File.Exists(file))
    {
        Log($"Using settings file {file}");
        builder.Configuration.AddJsonFile(file, optional: true, reloadOnChange: true);
    }
}

// Configure services with settings
ConfigureSettings<Setting>(builder);
ConfigureSettings<SDSettings>(builder);
//ConfigureSettings<HLSSettings>(builder);
ConfigureSettings<CommandProfileDict>(builder);
ConfigureSettings<OutputProfileDict>(builder);


void LoadAndSetSettings<TDict, TProfile>(string settingsFile, TDict defaultSetting)
    where TDict : IProfileDict<TProfile>
{
    // Load the settings
    var setting = SettingsHelper.GetSetting<TDict>(settingsFile);
    if (setting == null)
    {
        // If the setting is null, apply the entire default setting
        SettingsHelper.UpdateSetting(defaultSetting);
        return;
    }
    else
    {
        // If the setting is not null, apply the default setting for any missing profiles
        foreach (var defaultProfile in defaultSetting.Profiles)
        {
            if (!setting.Profiles.ContainsKey(defaultProfile.Key))
            {
                // Add missing entries
                setting.Profiles[defaultProfile.Key] = defaultProfile.Value;
            }
        }
    }
    //// If the setting is null or default, apply the entire default setting
    //if (EqualityComparer<TDict>.Default.Equals(setting, default(TDict)))
    //{
    //    SettingsHelper.UpdateSetting(defaultSetting);
    //    return;
    //}

    //// Ensure all entries from the default setting exist in the loaded setting
    //foreach (var defaultProfile in defaultSetting.Profiles)
    //{
    //    if (!setting.Profiles.ContainsKey(defaultProfile.Key))
    //    {
    //        // Add missing entries
    //        setting.Profiles[defaultProfile.Key] = defaultProfile.Value;
    //    }
    //}

    // Save the updated settings if changes were made
    SettingsHelper.UpdateSetting(setting);
}



// Helper method to load and validate settings
void LoadAndValidateSettings<T>(string settingsFile, object defaultSetting)
{
    var setting = SettingsHelper.GetSetting<T>(settingsFile);
    if (EqualityComparer<T>.Default.Equals(setting, default(T)))
    {
        SettingsHelper.UpdateSetting(defaultSetting);
    }
}

// Helper method to configure settings in services
void ConfigureSettings<T>(WebApplicationBuilder builder) where T : class
{
    builder.Services.Configure<T>(builder.Configuration);
}

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

builder.WebHost.UseUrls([.. urls]);

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
builder.Services.AddCustomPlayListServices();

var setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);

    builder.Services.AddWebUIServices(builder, setting?.EnableDBDebug?? false);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

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

WebApplication app = builder.Build();

app.UseForwardedHeaders();

var lifetime = app.Services.GetService<IHostApplicationLifetime>();
lifetime?.ApplicationStopping.Register(OnShutdown);

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

    //DirectoryHelper.EmptyDirectory(BuildInfo.HLSOutputFolder);

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

//app.UseHttpLogging();
app.UseMigrationsEndPoint();
app.UseSession();

//app.UseHangfireDashboard();

using (IServiceScope scope = app.Services.CreateScope())
{
       RepositoryContextInitializer initialiser = scope.ServiceProvider.GetRequiredService<RepositoryContextInitializer>();
    await initialiser.InitializeAsync().ConfigureAwait(false);
    if (app.Environment.IsDevelopment())
    {
        initialiser.TrySeed();
    }

    IImageDownloadService imageDownloadService = scope.ServiceProvider.GetRequiredService<IImageDownloadService>();
     imageDownloadService.Start();
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseRouting();
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

app.MapHub<StreamMasterHub>("/streammasterhub");//.RequireAuthorization("SignalR");

app.Run();

static string GetRoutePattern(Endpoint endpoint)
{
    RouteEndpoint? routeEndpoint = endpoint as RouteEndpoint;

    return routeEndpoint?.RoutePattern?.RawText is not null
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
