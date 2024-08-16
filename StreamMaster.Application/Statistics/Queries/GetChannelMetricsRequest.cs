using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelMetricsRequest() : IRequest<DataResponse<List<ChannelMetric>>>;

internal class GetChannelMetricsRequestHandler(IRepositoryWrapper repositoryWrapper, IHttpContextAccessor httpContextAccessor, IIconHelper iconHelper, IChannelService channelService, ICustomPlayListBuilder customPlayListBuilder, IChannelBroadcasterService channelBroadcasterService, IStreamBroadcasterService streamBroadcasterService)
    : IRequestHandler<GetChannelMetricsRequest, DataResponse<List<ChannelMetric>>>
{
    public async Task<DataResponse<List<ChannelMetric>>> Handle(GetChannelMetricsRequest request, CancellationToken cancellationToken)
    {
        List<IChannelBroadcaster> channelBroadcasters = channelBroadcasterService.GetChannelBroadcasters();
        List<IStreamBroadcaster> streamBroadcasters = streamBroadcasterService.GetStreamBroadcasters();

        List<ChannelMetric> dtos = [];

        List<string> smChannelIds = channelBroadcasters.SelectMany(a => a.ClientChannels.Keys).ToList();
        List<string> smStreamIds = channelBroadcasters.SelectMany(a => a.ClientStreams.Keys).ToList();

        List<string> smChannelIds2 = channelBroadcasters.SelectMany(a => a.ClientChannels.Keys).ToList();
        List<string> smStreamIds2 = channelBroadcasters.SelectMany(a => a.ClientStreams.Keys).ToList();
        List<string> smStreamIds3 = channelBroadcasters.Where(a => a.SMStreamInfo?.Id != null).Select(a => a.SMStreamInfo!.Id).ToList();

        smChannelIds.AddRange(smChannelIds2);
        smStreamIds.AddRange(smStreamIds2);
        smStreamIds.AddRange(smStreamIds3);

        List<SMChannel> smChannels = await repositoryWrapper.SMChannel.GetQuery(a => smChannelIds.Contains(a.Id.ToString())).ToListAsync(cancellationToken);
        List<SMStream> smStreams = await repositoryWrapper.SMStream.GetQuery(a => smStreamIds.Contains(a.Id)).ToListAsync(cancellationToken);
        string _baseUrl = httpContextAccessor.GetUrl();
        List<IClientConfiguration> clientConfigurations = channelService.GetClientStreamerConfigurations();

        foreach (IChannelBroadcaster channelBroadcaster in channelBroadcasters)
        {
            List<ClientChannelDto> channelDtos = [];

            int channelCount = 0;
            SMChannel? test = smChannels.Find(a => a.Id == channelBroadcaster.Id);
            string? logo = null;
            if (test?.Logo != null)
            {
                logo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, test.Logo, _baseUrl);
            }
            //channelDtos.Add(new ClientChannelDto()
            //{
            //    SMChannelId = test?.Id ?? channelCount++,
            //    Name = channelBroadcaster.Name,
            //    Logo = logo
            //});

            foreach (KeyValuePair<string, System.Threading.Channels.ChannelWriter<byte[]>> channel in channelBroadcaster.ClientChannels)
            {
                IClientConfiguration? config = clientConfigurations.Find(a => a.UniqueRequestId == channel.Key);
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
                    SMChannelId = test?.Id ?? channelCount++,
                    Name = channel.Key,
                    Logo = logo
                });
            }

            List<ClientStreamsDto> streamDtos = [];

            foreach (KeyValuePair<string, Stream> stream in channelBroadcaster.ClientStreams)
            {
                SMStream? test2 = smStreams.Find(a => a.Id == stream.Key);
                logo = null;
                if (test2?.Logo != null)
                {
                    logo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, test2.Logo, _baseUrl);
                }
                IClientConfiguration? config = clientConfigurations.Find(a => a.UniqueRequestId == stream.Key);
                streamDtos.Add(new ClientStreamsDto()
                {
                    ClientIPAddress = config?.ClientIPAddress,
                    ClientUserAgent = config?.ClientUserAgent,
                    SMStreamId = stream.Key,
                    Name = stream.Key,
                    Logo = logo
                });
            }

            logo = null;
            if (test?.Logo != null)
            {
                logo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, test.Logo, _baseUrl);
            }

            ChannelMetric dto = new()
            {
                Name = channelBroadcaster.Name,
                SourceName = channelBroadcaster.SourceName,
                //SMStreamInfo = d,
                ClientChannels = channelDtos,
                ClientStreams = streamDtos,
                ChannelItemBackLog = channelBroadcaster.ChannelItemBackLog,
                Metrics = channelBroadcaster.Metrics,
                IsFailed = channelBroadcaster.IsFailed,
                Id = channelBroadcaster.Id.ToString(),
                Logo = logo
            };

            //foreach (string channel in channelBroadcaster.ClientChannels.Keys)
            //{

            //}

            dtos.Add(dto);
        }

        foreach (IStreamBroadcaster channelBroadcaster in channelBroadcasters)
        {
            List<ClientChannelDto> channelDtos = [];

            int channelCount = 0;
            foreach (KeyValuePair<string, System.Threading.Channels.ChannelWriter<byte[]>> channel in channelBroadcaster.ClientChannels)
            {
                string? logo = null;
                SMChannel? test = smChannels.Find(a => a.Id.ToString() == channel.Key);
                if (test?.Logo != null)
                {
                    logo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, test.Logo, _baseUrl);
                }

                channelDtos.Add(new ClientChannelDto()
                {
                    SMChannelId = test?.Id ?? channelCount++,
                    Name = channel.Key,
                    Logo = logo
                });
            }

            List<ClientStreamsDto> streamDtos = [];

            foreach (KeyValuePair<string, Stream> stream in channelBroadcaster.ClientStreams)
            {
                string? logo = null;
                SMStream? test = smStreams.Find(a => a.Id == stream.Key);
                if (test?.Logo != null)
                {
                    logo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, test.Logo, _baseUrl);
                }

                streamDtos.Add(new ClientStreamsDto()
                {
                    SMStreamId = stream.Key,
                    Name = stream.Key,
                    Logo = logo
                });
            }

            SMStreamInfo? d = channelBroadcaster.SMStreamInfo?.DeepCopy() ?? null;
            d.Url = "Hidden";
            string id = d.Id;

            string? metricLogo = null;
            if (!channelBroadcaster.Id.Contains("://"))
            {
                metricLogo = customPlayListBuilder.GetCustomPlayListLogoFromFileName(channelBroadcaster.Id);
                metricLogo = iconHelper.GetIconUrl(EPGHelper.CustomPlayListId, metricLogo, _baseUrl);
            }
            else
            {
                SMStream? smStream = smStreams.Find(a => a.Id == d.Id);
                if (smStream != null)
                {
                    metricLogo = smStream.Logo;
                    //id = smStream.Id;
                }
            }


            ChannelMetric dto = new()
            {
                Name = channelBroadcaster.Name,
                SourceName = channelBroadcaster.SourceName,
                SMStreamInfo = d,
                ClientChannels = channelDtos,
                ClientStreams = streamDtos,
                ChannelItemBackLog = channelBroadcaster.ChannelItemBackLog,
                Metrics = channelBroadcaster.Metrics,
                IsFailed = channelBroadcaster.IsFailed,
                Id = id,
                Logo = metricLogo
            };

            dtos.Add(dto);
        }

        return DataResponse<List<ChannelMetric>>.Success(dtos);
    }
}
