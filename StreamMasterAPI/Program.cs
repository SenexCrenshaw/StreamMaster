using StreamMasterAPI;

using StreamMasterApplication;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure;
using StreamMasterInfrastructure.Persistence;

using System.Text;
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
    _urlBase =setting.BaseHostURL;

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

Console.WriteLine($"Writing {_urlBase} information to {indexFilePath}");

StringBuilder scriptBuilder = new();
scriptBuilder.AppendLine("window.StreamMaster = {");
scriptBuilder.AppendLine($"  baseHostURL: '{_urlBase}',");
scriptBuilder.AppendLine($"  hubName: 'streammasterhub',");
scriptBuilder.AppendLine($"  isDev: false,");
scriptBuilder.AppendLine("};");
File.WriteAllText(indexFilePath, scriptBuilder.ToString());

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        //.WithOrigins("http://localhost:3000")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});

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
.AddJsonOptions (options =>
{
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

WebApplication app = builder.Build();

app.UseHttpLogging();

_ = app.UseMigrationsEndPoint();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
    _ = app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for
    // production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

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

app.UseCors("CorsPolicy");

app.UseHealthChecks("/health");
//app.UseHttpsRedirection();

app.UseOpenApi();
app.UseSwaggerUi3();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
//app.UseHttpsRedirection();

app.MapHealthChecks("/healthz");
app.MapDefaultControllerRoute();

app.MapHub<StreamMasterHub>("/streammasterhub");

app.MapFallbackToFile("index.html");

app.Run();
