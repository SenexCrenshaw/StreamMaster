using Hangfire;
using Hangfire.Dashboard;

using StreamMasterAPI;

using StreamMasterApplication;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Logging;

using StreamMasterInfrastructureEF;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

FileUtil.SetupDirectories();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.AllowSynchronousIO = true;
    serverOptions.Limits.MaxRequestBodySize = null;
});

string settingFile = $"{BuildInfo.AppDataFolder}settings.json";

builder.Configuration.AddJsonFile(settingFile, true, false);
builder.Services.Configure<Setting>(builder.Configuration);

bool enableSsl = false;

string? sslCertPath = builder.Configuration["SSLCertPath"];
string? sslCertPassword = builder.Configuration["sslCertPassword"];

if (!bool.TryParse(builder.Configuration["EnableSSL"], out enableSsl))
{
}

List<string> urls = new() { "http://0.0.0.0:7095" };

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

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureEFServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();

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
});

WebApplication app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi3();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
    _ = app.UseMigrationsEndPoint();
    _ = app.UseForwardedHeaders();
}
else
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseForwardedHeaders();
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
    await initialiser.InitialiseAsync().ConfigureAwait(false);
    if (app.Environment.IsDevelopment())
    {
        initialiser.TrySeed();
    }
}

app.UseHealthChecks("/health");

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

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    //RecurringJob.AddOrUpdate("Hello World", () => Console.WriteLine("hello world"), Cron.Minutely);    
}
else
{
    _ = app.UseResponseCompression();
}

app.MapHealthChecks("/healthz");
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

app.MapHub<StreamMasterHub>("/streammasterhub").RequireAuthorization("SignalR");

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
