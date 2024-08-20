using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Streams.Channels;
using StreamMaster.Streams.Domain;
using StreamMaster.Streams.Factories;
using StreamMaster.Streams.Plugins;
using StreamMaster.Streams.Services;
using StreamMaster.Streams.Streams;

namespace StreamMaster.Streams;

public static class ConfigureServices
{
    public static IServiceCollection AddStreamsServices(this IServiceCollection services)
    {
        services.AddSingleton<IChannelManager, ChannelManager>();
        services.AddSingleton<IChannelService, ChannelService>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();

        //services.AddSingleton<IStreamStatisticService, StreamStatisticService>();
        //services.AddSingleton<IClientStatisticsManager, ClientStatisticsManager>();
        //services.AddSingleton<IChannelStreamingStatisticsManager, ChannelStreamingStatisticsManager>();
        //services.AddSingleton<IStreamStreamingStatisticsManager, StreamStreamingStatisticsManager>();
        //services.AddSingleton<IStreamManager, StreamManager>();
        services.AddTransient<ICommandExecutor, CommandExecutor>();
        services.AddTransient<ICustomPlayListStream, CustomPlayListStream>();
        services.AddTransient<IHTTPStream, HTTPStream>();
        services.AddTransient<ICommandStream, CommandStream>();
        services.AddTransient<IDubcer, Dubcer>();
        services.AddSingleton<ISourceBroadcasterService, SourceBroadcasterService>();
        services.AddSingleton<IHLSManager, HLSManager>();
        services.AddSingleton<IStreamTracker, StreamTracker>();
        services.AddSingleton<IAccessTracker, AccessTracker>();
        services.AddHostedService<InActiveStreamMonitor>();
        services.AddSingleton<ISwitchToNextStreamService, SwitchToNextStreamService>();
        services.AddSingleton<IM3U8Generator, M3U8Generator>();
        services.AddSingleton<IChannelBroadcasterService, ChannelBroadcasterService>();
        services.AddSingleton<IClientConfigurationService, ClientConfigurationService>();
        services.AddSingleton<IVideoInfoService, VideoInfoService>();
        services.AddTransient<IVideoCombiner, VideoCombiner>();
        services.AddSingleton<IVideoCombinerService, VideoCombinerService>();
        services.AddSingleton<IStreamLimitsService, StreamLimitsService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddSingleton<ICacheManager, CacheManager>();
        return services;
    }
}
