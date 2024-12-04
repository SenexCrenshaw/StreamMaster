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
        services.AddSingleton<HybridCacheManager<ScheduleService>>(provider =>
                  new HybridCacheManager<ScheduleService>(
                      provider.GetRequiredService<ILogger<HybridCacheManager<ScheduleService>>>(),
                      provider.GetRequiredService<IMemoryCache>()
                  )
              );


        services.AddSingleton<HybridCacheManager<MovieImages>>(provider =>
                  new HybridCacheManager<MovieImages>(
                      provider.GetRequiredService<ILogger<HybridCacheManager<MovieImages>>>(),
                      provider.GetRequiredService<IMemoryCache>()
                  )
              );

        services.AddSingleton<HybridCacheManager<SportsImages>>(provider =>
                    new HybridCacheManager<SportsImages>(
                        provider.GetRequiredService<ILogger<HybridCacheManager<SportsImages>>>(),
                        provider.GetRequiredService<IMemoryCache>()
                    )
                );

        services.AddSingleton<HybridCacheManager<SeriesImages>>(provider =>
                    new HybridCacheManager<SeriesImages>(
                        provider.GetRequiredService<ILogger<HybridCacheManager<SeriesImages>>>(),
                        provider.GetRequiredService<IMemoryCache>()
                    )
                );

        services.AddSingleton<HybridCacheManager<SeasonImages>>(provider =>
            new HybridCacheManager<SeasonImages>(
                provider.GetRequiredService<ILogger<HybridCacheManager<SeasonImages>>>(),
                provider.GetRequiredService<IMemoryCache>()
            )
        );

        services.AddSingleton<HybridCacheManager<EpisodeImages>>(provider =>
          new HybridCacheManager<EpisodeImages>(
              provider.GetRequiredService<ILogger<HybridCacheManager<EpisodeImages>>>(),
              provider.GetRequiredService<IMemoryCache>()
          )
      );

        services.AddSingleton<HybridCacheManager<GenericDescription>>(provider =>
          new HybridCacheManager<GenericDescription>(
              provider.GetRequiredService<ILogger<HybridCacheManager<GenericDescription>>>(),
              provider.GetRequiredService<IMemoryCache>()
          )
      );

        services.AddSingleton<HybridCacheManager<ProgramService>>(provider =>
          new HybridCacheManager<ProgramService>(
              provider.GetRequiredService<ILogger<HybridCacheManager<ProgramService>>>(),
              provider.GetRequiredService<IMemoryCache>(),
              useKeyBasedFiles: true
          ));


        services.AddSingleton<HybridCacheManager<LineupResult>>(provider =>
                    new HybridCacheManager<LineupResult>(
                        provider.GetRequiredService<ILogger<HybridCacheManager<LineupResult>>>(),
                        provider.GetRequiredService<IMemoryCache>(),
                        useKeyBasedFiles: true
                    )
                );

        services.AddSingleton<HybridCacheManager<CountryData>>(provider =>
                new HybridCacheManager<CountryData>(
                    provider.GetRequiredService<ILogger<HybridCacheManager<CountryData>>>(),
                    provider.GetRequiredService<IMemoryCache>(),
                    defaultKey: "Countries"
                )
            );

        services.AddSingleton<HybridCacheManager<Headend>>(provider =>
            new HybridCacheManager<Headend>(
                provider.GetRequiredService<ILogger<HybridCacheManager<Headend>>>(),
                provider.GetRequiredService<IMemoryCache>(),
                useKeyBasedFiles: true,
                defaultKey: "Headends"
            )
        );

        services.AddSingleton<HybridCacheManager<LineupPreviewChannel>>(provider =>
            new HybridCacheManager<LineupPreviewChannel>(
                provider.GetRequiredService<ILogger<HybridCacheManager<LineupPreviewChannel>>>(),
                provider.GetRequiredService<IMemoryCache>(),
                useKeyBasedFiles: true,
                defaultKey: "LineupPreviewChannels"
        )
        );


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