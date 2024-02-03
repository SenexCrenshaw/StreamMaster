
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Infrastructure.EF;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");


        _ = services.AddDbContextFactory<RepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)));


        _ = services.AddScoped<RepositoryContextInitializer>();

        //_ = services.AddScoped<ISortHelper<M3UFile>, SortHelper<M3UFile>>();
        //_ = services.AddScoped<ISortHelper<VideoStream>, SortHelper<VideoStream>>();
        //_ = services.AddScoped<ISortHelper<ChannelGroup>, SortHelper<ChannelGroup>>();
        //_ = services.AddScoped<ISortHelper<StreamGroup>, SortHelper<StreamGroup>>();
        //_ = services.AddScoped<ISortHelper<EPGFile>, SortHelper<EPGFile>>();
        _ = services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        return services;
    }
}
