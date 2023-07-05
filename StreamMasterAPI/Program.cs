using Microsoft.Extensions.FileProviders;

using StreamMasterAPI;

using StreamMasterApplication;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Persistence;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.AllowSynchronousIO = true;
    serverOptions.Limits.MaxRequestBodySize = null;
});

var appDataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.{Constants.AppName.ToLower()}{Path.DirectorySeparatorChar}";
var settingFile = $"{appDataFolder}settings.json";

builder.Configuration.AddJsonFile(settingFile, true, false);
builder.Services.Configure<Setting>(builder.Configuration);

var enableSsl = false;

var sslCertPath = builder.Configuration["SSLCertPath"];
var sslCertPassword = builder.Configuration["sslCertPassword"];

if (!bool.TryParse(builder.Configuration["EnableSSL"], out enableSsl))
{
}

var urls = new List<string> { "http://0.0.0.0:7095" };

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

    builder.WebHost.ConfigureKestrel(options =>
    options.ConfigureHttpsDefaults(configureOptions =>
       configureOptions.ServerCertificate = ValidateSslCertificate(Path.Combine(appDataFolder, sslCertPath), sslCertPassword)
    ));
}

// Add services to the container.
builder.Services.AddApplicationServices();
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
    app.UseForwardedHeaders();
}
else
{
    _ = app.UseExceptionHandler("/Error");
    app.UseForwardedHeaders();
    _ = app.UseHsts();
}

app.UseHttpLogging();

_ = app.UseMigrationsEndPoint();

app.UseSession();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<AppDbContextInitialiser>();

    await initialiser.InitialiseAsync().ConfigureAwait(false);
    if (app.Environment.IsDevelopment())
    {
        initialiser.TrySeed();
    }
}

//app.UseMiddleware<AuthMiddleware>();

app.UseHealthChecks("/health");
//app.UseHttpsRedirection();

app.UseDefaultFiles();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "devwwwroot")),
        RequestPath = "/devwwwroot"
    });
}
else
{
    app.UseStaticFiles();
}

app.UseRouting();
app.UseWebSockets();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevPolicy");
}
else
{
    app.UseCors();
}

//app.UseCors();
app.UseAuthentication();

//app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseResponseCompression();

app.MapHealthChecks("/healthz");
app.MapDefaultControllerRoute();

app.Map("/swagger", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.MapGet("/routes", async context =>
{
    var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();

    foreach (var endpoint in endpointDataSource.Endpoints)
    {
        var routePattern = GetRoutePattern(endpoint);

        await context.Response.WriteAsync($"Route: {routePattern}\n");
    }
});

app.MapHub<StreamMasterHub>("/streammasterhub").RequireAuthorization("SignalR");

app.Run();

string GetRoutePattern(Endpoint endpoint)
{
    var routeEndpoint = endpoint as RouteEndpoint;

    if (routeEndpoint is not null && routeEndpoint.RoutePattern is not null && routeEndpoint.RoutePattern.RawText is not null)
    {
        return routeEndpoint.RoutePattern.RawText;
    }

    return "<unknown>";
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
        if (ex.HResult == 0x2 || ex.HResult == 0x2006D080)
        {
            throw new Exception($"The SSL certificate file {cert} does not exist: {ex.Message}");
        }

        throw;
    }

    return certificate;
}