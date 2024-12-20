using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelMetricsRequest() : IRequest<DataResponse<List<ChannelMetric>>>;

internal class GetChannelMetricsRequestHandler(IRepositoryWrapper repositoryWrapper, IVideoInfoService videoInfoService, IHttpContextAccessor httpContextAccessor, ILogoService logoService, IChannelService channelService, ICustomPlayListBuilder customPlayListBuilder, IChannelBroadcasterService channelBroadcasterService, ISourceBroadcasterService sourceBroadcasterService)
    : IRequestHandler<GetChannelMetricsRequest, DataResponse<List<ChannelMetric>>>
{
    public async Task<DataResponse<List<ChannelMetric>>> Handle(GetChannelMetricsRequest request, CancellationToken cancellationToken)
    {
        List<IChannelBroadcaster> channelBroadcasters = channelBroadcasterService.GetChannelBroadcasters();
        List<ISourceBroadcaster> sourceBroadcasters = sourceBroadcasterService.GetStreamBroadcasters();

        List<ChannelMetric> dtos = [];

        List<int> smChannelIds = channelBroadcasters.ConvertAll(a => a.Id);
        List<string> smStreamIds = [.. sourceBroadcasters.ConvertAll(a => a.SMStreamInfo.Id)];

        List<SMChannel> smChannels = await repositoryWrapper.SMChannel.GetQuery(a => smChannelIds.Contains(a.Id)).ToListAsync(cancellationToken);
        List<SMStream> smStreams = await repositoryWrapper.SMStream.GetQuery(a => smStreamIds.Contains(a.Id)).ToListAsync(cancellationToken);
        string _baseUrl = httpContextAccessor.GetUrl();

        List<IClientConfiguration> clientConfigurations = channelService.GetClientStreamerConfigurations();

        foreach (IChannelBroadcaster channelBroadcaster in channelBroadcasters)
        {
            if (channelBroadcaster == null)
            {
                continue;
            }
            List<ClientChannelDto> channelDtos = [];

            IClientConfiguration? baseConfig = clientConfigurations.Find(a => a.SMChannel.Id == channelBroadcaster.SMChannel.Id);
            if (baseConfig == null)
            {
                continue;
            }
            SMChannelStreamLink bastCurrentStream = baseConfig.SMChannel.SMStreams.ToList()[baseConfig.SMChannel.CurrentRank];
            string currentStreamLogo = bastCurrentStream.SMStream!.Logo;// logoService.GetLogoUrl(bastCurrentStream.SMStream!.Logo, _baseUrl, SMStreamTypeEnum.Regular);
            string currentChannelLogo = baseConfig.SMChannel.Logo;// logoService.GetLogoUrl(baseConfig.SMChannel.Logo, _baseUrl, SMStreamTypeEnum.Regular);

            List<ClientStreamsDto> streamdtos = [];

            //
            foreach (KeyValuePair<string, IClientConfiguration> clientchannel in channelBroadcaster.Clients)
            {
                IClientConfiguration? config = clientConfigurations.Find(a => a.UniqueRequestId == clientchannel.Key);

                if (config == null)
                {
                    continue;
                }

                streamdtos.Add(new ClientStreamsDto()
                {
                    Metrics = config.Metrics,
                    ClientIPAddress = config?.ClientIPAddress,
                    ClientUserAgent = config?.ClientUserAgent,
                    SMChannelId = config.SMChannel.ChannelId,
                    Name = clientchannel.Key,
                    Logo = currentChannelLogo,
                });
            }

            ISourceBroadcaster? test = sourceBroadcasters.Find(a => a.SMStreamInfo.Url == channelBroadcaster.SMStreamInfo.Url);

            //foreach (var client in channelBroadcaster.Clients)
            //{
            //    streamdtos.Add(new ClientStreamsDto
            //    {
            //        Metrics = client.Value.Metrics,
            //        ClientIPAddress = client.Value.ClientIPAddress,
            //        ClientUserAgent = client.Value.ClientUserAgent,
            //        SMChannelId = channelBroadcaster.Id,
            //        Name = client.Key,
            //        Logo = currentStreamLogo,
            //    });
            //}

            ChannelMetric dto = new()
            {
                Name = channelBroadcaster.SourceName,
                SourceName = channelBroadcaster.SourceName,
                ClientStreams = streamdtos,
                IsFailed = channelBroadcaster.IsFailed,
                Id = channelBroadcaster.Id.ToString(),
                Logo = currentStreamLogo,

            };
            if (test is not null)
            {
                dto.SMStreamInfo = test.SMStreamInfo;
                if (test.Metrics is not null)
                {
                    dto.Metrics = test.Metrics;
                }

                VideoInfo? info = videoInfoService.GetVideoInfo(test.SMStreamInfo.Id);
                if (info != null)
                {
                    VideoInfoDto videoInfoDto = new(info);
                    dto.VideoInfo = videoInfoDto?.JsonOutput;
                }

            }

            dtos.Add(dto);
        }

        //foreach (ISourceBroadcaster sourceBroadcaster in sourceBroadcasters)
        //{
        //    if (sourceBroadcaster.SMStreamInfo == null)
        //    {
        //        continue;
        //    }

        //    SMStreamInfo cuurentStreamInfo = sourceBroadcaster.SMStreamInfo.DeepCopy();
        //    List<IChannelBroadcaster> test = channelBroadcasterService.GetChannelBroadcasters().Where(a => a.SMStreamInfo.Url == sourceBroadcaster.Id).ToList();

        //    List<ClientChannelDto> channelDtos = [];

        //    //IClientConfiguration? baseConfig = clientConfigurations.Find(a => a.SMChannel.Id == sourceBroadcaster.);
        //    //if (baseConfig == null)
        //    //{
        //    //    continue;
        //    //}

        //    //SMChannelStreamLink bastCurrentStream = baseConfig.SMChannel.SMStreams.ToList()[baseConfig.SMChannel.CurrentRank];
        //    //string currentStreamLogo = logoService.GetLogoUrl(bastCurrentStream.SMStream.SMLogoUrl, _baseUrl);

        //    //int channelCount = 0;

        //    string currentChannelLogo = "";

        //    List<ClientStreamsDto> streamDtos = [];

        //    cuurentStreamInfo.Url = "Hidden";
        //    string id = cuurentStreamInfo.Id;

        //    if (!sourceBroadcaster.Id.Contains("://"))
        //    {
        //        string? newCurrentChannelLogo = customPlayListBuilder.GetCustomPlayListLogoFromFileName(sourceBroadcaster.Id);
        //        //if (string.IsNullOrEmpty(metricLogo))
        //        //{
        //        //    SMChannel? test = smChannels.Find(a => a.Id.ToString() == sourceBroadcaster.Id);
        //        //    metricLogo = test?.SMLogoUrl ?? "";
        //        //}
        //        if (!string.IsNullOrEmpty(newCurrentChannelLogo))
        //        {
        //            logoService.AddLogoToCache(id, currentChannelLogo);
        //        }
        //    }
        //    //else
        //    //{
        //    //    SMStream? smStream = smStreams.Find(a => a.Id == cuurentStreamInfo.Id);
        //    //    if (smStream != null)
        //    //    {
        //    //        metricLogo = smStream.SMLogoUrl;
        //    //        //id = smStream.Id;
        //    //    }
        //    //}

        //    VideoInfo? info = videoInfoService.GetVideoInfo(id);
        //    VideoInfoDto? videoInfoDto = null;
        //    if (info != null)
        //    {
        //        videoInfoDto = new(info);
        //    }

        //    ChannelMetric dto = new()
        //    {

        //        //FIXSD
        //        //Name = sourceBroadcaster.Name,
        //        //SourceName = sourceBroadcaster.SourceName,
        //        SMStreamInfo = cuurentStreamInfo,
        //        ClientChannels = channelDtos,
        //        ClientStreams = streamDtos,
        //        //ChannelItemBackLog = sourceBroadcaster.ChannelItemBackLog,
        //        //Metrics = sourceBroadcaster.Metrics,
        //        IsFailed = sourceBroadcaster.IsFailed,
        //        Id = id,
        //        Logo = currentChannelLogo,
        //        VideoInfo = videoInfoDto?.JsonOutput
        //    };

        //    dtos.Add(dto);
        //}

        return DataResponse<List<ChannelMetric>>.Success(dtos);
    }
}