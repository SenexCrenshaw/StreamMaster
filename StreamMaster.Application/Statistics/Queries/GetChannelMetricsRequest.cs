using System.Collections.Concurrent;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelMetricsRequest() : IRequest<DataResponse<List<ChannelMetric>>>;

internal class GetChannelMetricsRequestHandler(
    IVideoInfoService videoInfoService,
    IHttpContextAccessor httpContextAccessor,
    IChannelService channelService,
    IChannelBroadcasterService channelBroadcasterService,
    ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetChannelMetricsRequest, DataResponse<List<ChannelMetric>>>
{
    public Task<DataResponse<List<ChannelMetric>>> Handle(GetChannelMetricsRequest request, CancellationToken cancellationToken)
    {
        List<IChannelBroadcaster> channelBroadcasters = channelBroadcasterService.GetChannelBroadcasters();
        if (channelBroadcasters.Count == 0)
        {
            return Task.FromResult(DataResponse<List<ChannelMetric>>.Success([]));
        }

        string baseUrl = httpContextAccessor.GetUrl();

        Dictionary<string, IClientConfiguration> clientConfigDict = channelService.GetClientStreamerConfigurations()
            .ToDictionary(a => a.UniqueRequestId);

        Dictionary<int, SMChannelDto> smChannelDict = GetSMChannelDictionary(clientConfigDict.Values);

        Dictionary<string, ISourceBroadcaster> sourceBroadcasters = sourceBroadcasterService.GetStreamBroadcasters()
            .ToDictionary(sb => sb.Id);

        List<ChannelMetric> channelMetrics = BuildChannelMetrics(channelBroadcasters, smChannelDict, sourceBroadcasters, clientConfigDict);

        return Task.FromResult(DataResponse<List<ChannelMetric>>.Success(channelMetrics));
    }

    private static Dictionary<int, SMChannelDto> GetSMChannelDictionary(IEnumerable<IClientConfiguration> clientConfigs)
    {
        return clientConfigs
            .Select(a => a.SMChannel)
            .GroupBy(smChannel => smChannel.Id)
            .ToDictionary(group => group.Key, group => group.First());
    }

    private List<ChannelMetric> BuildChannelMetrics(
        List<IChannelBroadcaster> channelBroadcasters,
        Dictionary<int, SMChannelDto> smChannelDict,
        Dictionary<string, ISourceBroadcaster> sourceBroadcasters,
        Dictionary<string, IClientConfiguration> clientConfigDict)
    {
        List<ChannelMetric> channelMetrics = [];

        foreach (IChannelBroadcaster channelBroadcaster in channelBroadcasters)
        {
            if (channelBroadcaster == null ||
                !smChannelDict.TryGetValue(channelBroadcaster.SMChannel.Id, out SMChannelDto? smChannel))
            {
                continue;
            }

            SMStream? currentStream = smChannel.SMStreams.FirstOrDefault(a => a.Rank == smChannel.CurrentRank)?.SMStream;
            if (currentStream == null)
            {
                continue;
            }

            List<ClientStreamsDto> streamDtos = BuildClientStreamDtos(channelBroadcaster, clientConfigDict, smChannel, currentStream);

            ChannelMetric channelMetric = BuildChannelMetric(channelBroadcaster, smChannel, currentStream, streamDtos, sourceBroadcasters);

            channelMetrics.Add(channelMetric);
        }

        return channelMetrics;
    }

    private static List<ClientStreamsDto> BuildClientStreamDtos(
    IChannelBroadcaster channelBroadcaster,
    Dictionary<string, IClientConfiguration> clientConfigDict,
    SMChannelDto smChannel,
    SMStream currentStream)
    {
        ConcurrentBag<ClientStreamsDto> streamDtos = []; // Use ConcurrentBag for thread-safe additions

        Parallel.ForEach(channelBroadcaster.Clients, clientChannel =>
        {
            if (clientConfigDict.TryGetValue(clientChannel.Key, out IClientConfiguration? clientConfig))
            {
                ClientStreamsDto dto = new()
                {
                    Metrics = clientConfig.Metrics,
                    ClientIPAddress = clientConfig.ClientIPAddress,
                    ClientUserAgent = clientConfig.ClientUserAgent,
                    SMChannelId = clientConfig.SMChannel.ChannelId,
                    Name = clientChannel.Key,
                    ChannelLogo = smChannel.Logo,
                    StreamLogo = currentStream.Logo
                };

                streamDtos.Add(dto); // Thread-safe addition to ConcurrentBag
            }
        });

        return [.. streamDtos]; // Convert ConcurrentBag to List before returning
    }

    private ChannelMetric BuildChannelMetric(
        IChannelBroadcaster channelBroadcaster,
        SMChannelDto smChannel,
        SMStream currentStream,
        List<ClientStreamsDto> streamDtos,
        Dictionary<string, ISourceBroadcaster> sourceBroadcasters)
    {
        ChannelMetric channelMetric = new()
        {
            Name = channelBroadcaster.SourceName,
            SourceName = channelBroadcaster.SourceName,
            ClientStreams = streamDtos,
            IsFailed = channelBroadcaster.IsFailed,
            Id = channelBroadcaster.Id.ToString(),
            ChannelLogo = smChannel.Logo,
            StreamLogo = currentStream.Logo
        };

        if (channelBroadcaster.SMStreamInfo is not null &&
            sourceBroadcasters.TryGetValue(channelBroadcaster.SMStreamInfo.Url, out ISourceBroadcaster? sourceBroadcaster))
        {
            channelMetric.SMStreamInfo = channelBroadcaster.SMStreamInfo;

            if (sourceBroadcaster.Metrics is not null)
            {
                channelMetric.Metrics = sourceBroadcaster.Metrics;
            }

            VideoInfo? videoInfo = videoInfoService.GetVideoInfo(sourceBroadcaster.Id);
            if (videoInfo is not null)
            {
                channelMetric.VideoInfo = new VideoInfoDto(videoInfo).JsonOutput;
            }
        }

        return channelMetric;
    }
}
