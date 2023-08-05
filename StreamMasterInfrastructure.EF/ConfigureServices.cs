using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterDomain.Common;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

using StreamMasterInfrastructure.EF.Helpers;

namespace StreamMasterInfrastructure.EF;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {
        Setting setting = FileUtil.GetSetting();

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");

        services.AddDbContext<RepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)));
        services.AddScoped<RepositoryContextInitializer>();

        services.AddScoped<ISortHelper<M3UFile>, SortHelper<M3UFile>>();
        services.AddScoped<ISortHelper<VideoStream>, SortHelper<VideoStream>>();
        services.AddScoped<ISortHelper<ChannelGroup>, SortHelper<ChannelGroup>>();
        services.AddScoped<ISortHelper<StreamGroup>, SortHelper<StreamGroup>>();
        services.AddScoped<ISortHelper<EPGFile>, SortHelper<EPGFile>>();
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        return services;
    }
}
