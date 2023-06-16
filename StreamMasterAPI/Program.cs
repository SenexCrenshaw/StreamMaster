using Microsoft.Extensions.FileProviders;

using StreamMasterAPI;

using StreamMasterApplication;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Persistence;

using System.Text.Json.Serialization;

Setting setting = FileUtil.GetSetting();
if (!setting.BaseHostURL.EndsWith('/'))
{
    setting.BaseHostURL += '/';
    FileUtil.UpdateSetting(setting);
}

string? _urlBase = Environment.GetEnvironmentVariable("STREAMMASTER_BASEHOSTURL");

if (string.IsNullOrEmpty(_urlBase))
{
    _urlBase = setting.BaseHostURL;
}

if (!_urlBase.EndsWith('/'))
{
    _urlBase += '/';
}

if (_urlBase != setting.BaseHostURL)
{
    setting.BaseHostURL = _urlBase;
    FileUtil.UpdateSetting(setting);
}

string indexFilePath = Path.GetFullPath("wwwroot") + Path.DirectorySeparatorChar + "initialize.js";
var apikey = setting.ApiKey;

//Console.WriteLine($"Writing {_urlBase} information to {indexFilePath}");
//var settingDto = new SettingDto();
//StringBuilder scriptBuilder = new();
//scriptBuilder.AppendLine("window.StreamMaster = {");
//scriptBuilder.AppendLine($"  apiKey: '{setting.ApiKey}',");
//scriptBuilder.AppendLine($"  apiRoot: '/api',");
////if (!string.IsNullOrEmpty(setting.APIPassword) && !string.IsNullOrEmpty(setting.APIUserName))
////{
////    scriptBuilder.AppendLine($"  apiPassword: '{setting.APIPassword}',");
////    scriptBuilder.AppendLine($"  apiUserName: '{setting.APIUserName}',");
////}
//scriptBuilder.AppendLine($"  baseHostURL: '{_urlBase}',");
//scriptBuilder.AppendLine($"  hubName: 'streammasterhub',");
//scriptBuilder.AppendLine($"  isDev: false,");
//scriptBuilder.AppendLine($"  requiresAuth: {(!string.IsNullOrEmpty(setting.AdminPassword) && !string.IsNullOrEmpty(setting.AdminUserName)).ToString().ToLower()},");
//scriptBuilder.AppendLine($"  urlBase: '{settingDto.UrlBase}',");
//scriptBuilder.AppendLine($"  version: '{settingDto.Version}',");
//scriptBuilder.AppendLine("};");
//File.WriteAllText(indexFilePath, scriptBuilder.ToString());

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("CorsPolicy", builder => builder
//        //.WithOrigins("http://localhost:3000")
//        .AllowAnyOrigin()
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        );
//});

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

//app.UseResponseCompression();

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
app.UseWebSockets(); app.UseOpenApi();
app.UseSwaggerUi3();

//if (app.Environment.IsDevelopment())
//{
//    app.UseCors("DevPolicy");
//}
//else
//{
//    app.UseCors();
//}

app.UseCors();
app.UseAuthentication();

//app.UseHttpsRedirection();
app.UseAuthorization();


app.MapHealthChecks("/healthz");
app.MapDefaultControllerRoute();

app.UseEndpoints(endpoints =>
{
    endpoints.Map("/swagger", context =>
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });

    endpoints.MapGet("/routes", async context =>
    {
        var endpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();

        foreach (var endpoint in endpointDataSource.Endpoints)
        {
            var routePattern = GetRoutePattern(endpoint);

            await context.Response.WriteAsync($"Route: {routePattern}\n");
        }
    });

    app.MapHub<StreamMasterHub>("/streammasterhub").RequireAuthorization("SignalR");
});

//app.MapFallbackToFile("index.html");

app.Run();

string GetRoutePattern(Endpoint endpoint)
{
    var routeEndpoint = endpoint as RouteEndpoint;

    if (routeEndpoint is not null)
    {
        return routeEndpoint.RoutePattern.RawText;
    }

    return "<unknown>";
}
