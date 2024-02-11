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
        _ = services.AddSingleton<IStreamSwitcher, StreamSwitcher>();
        _ = services.AddSingleton<IChannelService, ChannelService>();
        _ = services.AddSingleton<IProxyFactory, ProxyFactory>();
        _ = services.AddSingleton<IClientStreamerManager, ClientStreamerManager>();
        _ = services.AddSingleton<IStreamHandlerFactory, StreamHandlerFactory>();
        _ = services.AddSingleton<IStreamStatisticService, StreamStatisticService>();
        _ = services.AddSingleton<IStatisticsManager, ClientStatisticsManager>();
        _ = services.AddSingleton<IInputStatisticsManager, InputStatisticsManager>();
        _ = services.AddSingleton<IStreamManager, StreamManager>();

        return services;
    }
}
