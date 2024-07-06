using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Streams.Channels;
using StreamMaster.Streams.Clients;
using StreamMaster.Streams.Factories;
using StreamMaster.Streams.Statistics;
using StreamMaster.Streams.Streams;

namespace StreamMaster.Streams;

public static class ConfigureServices
{
    public static IServiceCollection AddStreamsServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<IChannelManager, ChannelManager>();
        _ = services.AddSingleton<IChannelService, ChannelService>();
        _ = services.AddSingleton<IProxyFactory, ProxyFactory>();
        _ = services.AddSingleton<IClientStreamerManager, ClientStreamerManager>();
        _ = services.AddSingleton<IStreamHandlerFactory, StreamHandlerFactory>();
        _ = services.AddSingleton<IStreamStatisticService, StreamStatisticService>();
        _ = services.AddSingleton<IClientStatisticsManager, ClientStatisticsManager>();
        _ = services.AddSingleton<IChannelStreamingStatisticsManager, ChannelStreamingStatisticsManager>();
        _ = services.AddSingleton<IStreamStreamingStatisticsManager, StreamStreamingStatisticsManager>();
        _ = services.AddSingleton<IStreamManager, StreamManager>();
        //_ = services.AddScoped<IOverlayStreamGenerator, OverlayStreamGenerator>();
        _ = services.AddSingleton<IHLSManager, HLSManager>();

        return services;
    }
}
