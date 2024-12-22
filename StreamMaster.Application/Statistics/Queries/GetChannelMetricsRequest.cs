using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelMetricsRequest() : IRequest<DataResponse<List<ChannelMetric>>>;

internal class GetChannelMetricsRequestHandler(IRepositoryWrapper repositoryWrapper, IVideoInfoService videoInfoService, IHttpContextAccessor httpContextAccessor, IChannelService channelService, IChannelBroadcasterService channelBroadcasterService, ISourceBroadcasterService sourceBroadcasterService)
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

        //Dictionary<int, SMStreamInfo> streamSMStreamInfoDict = channelBroadcasters.Where(a => a?.SMStreamInfo is not null).ToDictionary(a => a.Id, a => a.SMStreamInfo!);

        //List<string> streamIDs = [.. streamSMStreamInfoDict.Values.Select(a => a.Id)];
        //Dictionary<string, string> streamLogos = repositoryWrapper.SMStream.GetQuery().Where(a => streamIDs.Contains(a.Id)).ToDictionary(a => a.Id, a => a.ChannelLogo);

        Dictionary<int, SMChannelDto> smChannelDict = clientConfigDict.Values
      .Select(a => a.SMChannel)
      .GroupBy(smChannel => smChannel.Id)
      .ToDictionary(group => group.Key, group => group.First());

        Dictionary<string, ISourceBroadcaster> sourceBroadcasters = sourceBroadcasterService.GetStreamBroadcasters()
            .ToDictionary(sb => sb.Id);

        List<ChannelMetric> channelMetrics = [];

        foreach (IChannelBroadcaster? channelBroadcaster in channelBroadcasters)
        {
            if (channelBroadcaster is null || !smChannelDict.TryGetValue(channelBroadcaster.SMChannel.Id, out SMChannelDto? smChannel))
            {
                continue;
            }

            SMChannelStreamLink? currentStream = smChannel.SMStreams.ElementAtOrDefault(smChannel.CurrentRank);
            if (currentStream?.SMStream is null)
            {
                continue;
            }

            List<ClientStreamsDto> streamDtos = [];
            foreach (KeyValuePair<string, IClientConfiguration> clientChannel in channelBroadcaster.Clients)
            {
                if (clientConfigDict.TryGetValue(clientChannel.Key, out IClientConfiguration? clientConfig))
                {

                    streamDtos.Add(new ClientStreamsDto
                    {
                        Metrics = clientConfig.Metrics,
                        ClientIPAddress = clientConfig.ClientIPAddress,
                        ClientUserAgent = clientConfig.ClientUserAgent,
                        SMChannelId = clientConfig.SMChannel.ChannelId,
                        Name = clientChannel.Key,
                        ChannelLogo = smChannel.Logo,
                        StreamLogo = currentStream.SMStream.Logo
                    });
                }
            }

            ChannelMetric channelMetric = new()
            {
                Name = channelBroadcaster.SourceName,
                SourceName = channelBroadcaster.SourceName,
                ClientStreams = streamDtos,
                IsFailed = channelBroadcaster.IsFailed,
                Id = channelBroadcaster.Id.ToString(),
                ChannelLogo = smChannel.Logo,
                StreamLogo = currentStream.SMStream.Logo,
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

            channelMetrics.Add(channelMetric);
        }

        return Task.FromResult(DataResponse<List<ChannelMetric>>.Success(channelMetrics));
    }
}