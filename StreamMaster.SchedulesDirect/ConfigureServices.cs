using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Cache;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Images;
using StreamMaster.SchedulesDirect.Services;

namespace StreamMaster.SchedulesDirect;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectServices(this IServiceCollection services)
    {
        services.AddSingleton<SMCacheManager<ScheduleService>>(provider =>
                  new SMCacheManager<ScheduleService>(
                      provider.GetRequiredService<ILogger<ScheduleService>>(),
                      provider.GetRequiredService<IMemoryCache>()
                  )
              );

        services.AddSingleton<SMCacheManager<MovieImages>>(provider =>
                  new SMCacheManager<MovieImages>(
                      provider.GetRequiredService<ILogger<MovieImages>>(),
                      provider.GetRequiredService<IMemoryCache>()
                  )
              );

        services.AddSingleton<SMCacheManager<SportsImages>>(provider =>
                    new SMCacheManager<SportsImages>(
                        provider.GetRequiredService<ILogger<SportsImages>>(),
                        provider.GetRequiredService<IMemoryCache>()
                    )
                );

        services.AddSingleton<SMCacheManager<SeriesImages>>(provider =>
                    new SMCacheManager<SeriesImages>(
                        provider.GetRequiredService<ILogger<SeriesImages>>(),
                        provider.GetRequiredService<IMemoryCache>()
                    )
                );

        services.AddSingleton<SMCacheManager<SeasonImages>>(provider =>
            new SMCacheManager<SeasonImages>(
                provider.GetRequiredService<ILogger<SeasonImages>>(),
                provider.GetRequiredService<IMemoryCache>()
            )
        );

        services.AddSingleton<SMCacheManager<EpisodeImages>>(provider =>
          new SMCacheManager<EpisodeImages>(
              provider.GetRequiredService<ILogger<EpisodeImages>>(),
              provider.GetRequiredService<IMemoryCache>()
          )
      );

        services.AddSingleton<SMCacheManager<GenericDescription>>(provider =>
          new SMCacheManager<GenericDescription>(
              provider.GetRequiredService<ILogger<GenericDescription>>(),
              provider.GetRequiredService<IMemoryCache>()
          )
      );

        services.AddSingleton<SMCacheManager<ProgramService>>(provider =>
          new SMCacheManager<ProgramService>(
              provider.GetRequiredService<ILogger<ProgramService>>(),
              provider.GetRequiredService<IMemoryCache>(),
              useKeyBasedFiles: true
          ));

        services.AddSingleton<SMCacheManager<LineupResult>>(provider =>
                    new SMCacheManager<LineupResult>(
                        provider.GetRequiredService<ILogger<LineupResult>>(),
                        provider.GetRequiredService<IMemoryCache>(),
                        useKeyBasedFiles: true
                    )
                );

        services.AddSingleton<SMCacheManager<CountryData>>(provider =>
                new SMCacheManager<CountryData>(
                    provider.GetRequiredService<ILogger<CountryData>>(),
                    provider.GetRequiredService<IMemoryCache>(),
                    defaultKey: "Countries"
                )
            );

        services.AddSingleton<SMCacheManager<Headend>>(provider =>
            new SMCacheManager<Headend>(
                provider.GetRequiredService<ILogger<Headend>>(),
                provider.GetRequiredService<IMemoryCache>(),
                useKeyBasedFiles: true,
                defaultKey: "Headends"
            )
        );

        services.AddSingleton<SMCacheManager<LineupPreviewChannel>>(provider =>
            new SMCacheManager<LineupPreviewChannel>(
                provider.GetRequiredService<ILogger<LineupPreviewChannel>>(),
                provider.GetRequiredService<IMemoryCache>(),
                useKeyBasedFiles: true,
                defaultKey: "LineupPreviewChannels"
        )
        );

        services.AddSingleton<IProgramRepository, ProgramRepository>();

        _ = services.AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>();

        _ = services.AddScoped<ISDXMLTVBuilder, SDXMLTVBuilder>();

        _ = services.AddScoped<IScheduleService, ScheduleService>();
        _ = services.AddScoped<IDescriptionService, DescriptionService>();
        _ = services.AddScoped<ILineupService, LineupService>();
        _ = services.AddScoped<IProgramService, ProgramService>();
        _ = services.AddScoped<ISchedulesDirect, SchedulesDirect>();
        _ = services.AddScoped<IMovieImages, MovieImages>();
        _ = services.AddScoped<ISeriesImages, SeriesImages>();
        _ = services.AddScoped<ISeasonImages, SeasonImages>();
        _ = services.AddScoped<ISportsImages, SportsImages>();
        _ = services.AddScoped<IEpisodeImages, EpisodeImages>();

        _ = services.AddTransient<IXmltvChannelBuilder, XmltvChannelBuilder>();
        _ = services.AddTransient<IXmltvProgramBuilder, XmltvProgramBuilder>();
        _ = services.AddTransient<IDataPreparationService, DataPreparationService>();

        return services;
    }
}