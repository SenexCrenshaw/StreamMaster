using Microsoft.AspNetCore.Http;

using StreamMaster.Streams.Domain;

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
        List<string> smStreamIds = sourceBroadcasters.ConvertAll(a => a.SMStreamInfo.Id).ToList();

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
            string currentStreamLogo = logoService.GetLogoUrl(bastCurrentStream.SMStream.Logo, _baseUrl);
            string currentChannelLogo = logoService.GetLogoUrl(baseConfig.SMChannel.Logo, _baseUrl);

            foreach (KeyValuePair<string, TrackedChannel> clientChannel in channelBroadcaster.ClientChannels)
            {
                IClientConfiguration? config = clientConfigurations.Find(a => a.UniqueRequestId == clientChannel.Key);

                StreamHandlerMetrics? metric = null;
                if (config?.ClientStream != null)
                {
                    metric = config.ClientStream.Metrics;
                }

                channelDtos.Add(new ClientChannelDto()
                {
                    Metrics = metric,
                    ClientIPAddress = config?.ClientIPAddress,
                    ClientUserAgent = config?.ClientUserAgent,
                    SMChannelId = channelBroadcaster.Id,
                    Name = clientChannel.Key,
                    Logo = currentChannelLogo,
                    TotalBytesInBuffer = clientChannel.Value.TotalBytesInBuffer
                });
            }

            List<ClientStreamsDto> streamDtos = [];


            ChannelMetric dto = new()
            {
                Name = channelBroadcaster.Name,
                SourceName = channelBroadcaster.SourceName,
                ClientChannels = channelDtos,
                ClientStreams = streamDtos,
                ChannelItemBackLog = channelBroadcaster.ChannelItemBackLog,
                Metrics = channelBroadcaster.Metrics,
                IsFailed = channelBroadcaster.IsFailed,
                Id = channelBroadcaster.Id.ToString(),
                Logo = currentStreamLogo,
                TotalBytesInBuffer = channelBroadcaster.Channel.TotalBytesInBuffer
            };

            dtos.Add(dto);
        }

        foreach (ISourceBroadcaster sourceBroadcaster in sourceBroadcasters)
        {
            if (sourceBroadcaster.SMStreamInfo == null)
            {
                continue;
            }



            SMStreamInfo cuurentStreamInfo = sourceBroadcaster.SMStreamInfo.DeepCopy();
            List<ClientChannelDto> channelDtos = [];

            //IClientConfiguration? baseConfig = clientConfigurations.Find(a => a.SMChannel.Id == sourceBroadcaster.);
            //if (baseConfig == null)
            //{
            //    continue;
            //}

            //SMChannelStreamLink bastCurrentStream = baseConfig.SMChannel.SMStreams.ToList()[baseConfig.SMChannel.CurrentRank];
            //string currentStreamLogo = logoService.GetLogoUrl(bastCurrentStream.SMStream.Logo, _baseUrl);


            //int channelCount = 0;

            string currentChannelLogo = "";

            foreach (KeyValuePair<string, TrackedChannel> channel in sourceBroadcaster.ClientChannels)
            {
                SMChannel? smChannel = smChannels.Find(a => a.Id.ToString() == channel.Key);
                if (smChannel == null)
                {
                    continue;
                }

                currentChannelLogo = logoService.GetLogoUrl(smChannel.Logo, _baseUrl);


                channelDtos.Add(new ClientChannelDto()
                {
                    SMChannelId = smChannel.Id,
                    Name = channel.Key,
                    Logo = currentChannelLogo
                });


            }

            List<ClientStreamsDto> streamDtos = [];


            cuurentStreamInfo.Url = "Hidden";
            string id = cuurentStreamInfo.Id;


            if (!sourceBroadcaster.Id.Contains("://"))
            {

                currentChannelLogo = customPlayListBuilder.GetCustomPlayListLogoFromFileName(sourceBroadcaster.Id);
                //if (string.IsNullOrEmpty(metricLogo))
                //{
                //    SMChannel? test = smChannels.Find(a => a.Id.ToString() == sourceBroadcaster.Id);
                //    metricLogo = test?.Logo ?? "";
                //}
                currentChannelLogo = logoService.GetLogoUrl(currentChannelLogo, _baseUrl);
            }
            //else
            //{
            //    SMStream? smStream = smStreams.Find(a => a.Id == cuurentStreamInfo.Id);
            //    if (smStream != null)
            //    {

            //        metricLogo = smStream.Logo;
            //        //id = smStream.Id;
            //    }
            //}

            VideoInfo? info = videoInfoService.GetVideoInfo(id);
            VideoInfoDto? videoInfoDto = null;
            if (info != null)
            {
                videoInfoDto = new(info);

            }


            ChannelMetric dto = new()
            {
                Name = sourceBroadcaster.Name,
                SourceName = sourceBroadcaster.SourceName,
                SMStreamInfo = cuurentStreamInfo,
                ClientChannels = channelDtos,
                ClientStreams = streamDtos,
                ChannelItemBackLog = sourceBroadcaster.ChannelItemBackLog,
                Metrics = sourceBroadcaster.Metrics,
                IsFailed = sourceBroadcaster.IsFailed,
                Id = id,
                Logo = currentChannelLogo,
                VideoInfo = videoInfoDto?.JsonOutput,
                TotalBytesInBuffer = sourceBroadcaster.Channel.TotalBytesInBuffer

            };

            dtos.Add(dto);
        }

        return DataResponse<List<ChannelMetric>>.Success(dtos);
    }
}
