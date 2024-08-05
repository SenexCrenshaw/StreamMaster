namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelDistributorsRequest() : IRequest<DataResponse<List<ChannelDistributorDto>>>;

internal class GetChannelDistributorsRequestHandler(IRepositoryWrapper repositoryWrapper, IChannelBroadcasterService channelDistributorService)
    : IRequestHandler<GetChannelDistributorsRequest, DataResponse<List<ChannelDistributorDto>>>
{
    public async Task<DataResponse<List<ChannelDistributorDto>>> Handle(GetChannelDistributorsRequest request, CancellationToken cancellationToken)
    {
        List<IChannelBroadcaster> channelDistributors = channelDistributorService.GetChannelBroadcasters();
        List<ChannelDistributorDto> dtos = [];

        List<string> smChannelIds = channelDistributors.SelectMany(a => a.ClientChannels.Keys).ToList();
        List<string> smStreamIds = channelDistributors.SelectMany(a => a.ClientStreams.Keys).ToList();

        List<SMChannel> smChannels = await repositoryWrapper.SMChannel.GetQuery(a => smChannelIds.Contains(a.Id.ToString())).ToListAsync(cancellationToken);
        List<SMStream> smStreams = await repositoryWrapper.SMStream.GetQuery(a => smStreamIds.Contains(a.Id)).ToListAsync(cancellationToken);

        foreach (IChannelBroadcaster channelDistributor in channelDistributors)
        {
            List<ClientChannelDto> channelDtos = [];

            int channelCount = 0;
            foreach (KeyValuePair<string, System.Threading.Channels.ChannelWriter<byte[]>> channel in channelDistributor.ClientChannels)
            {
                SMChannel? test = smChannels.FirstOrDefault(a => a.Id.ToString() == channel.Key);

                channelDtos.Add(new ClientChannelDto()
                {
                    SMChannelId = test?.Id ?? channelCount++,
                    Name = channel.Key,
                    Logo = test?.Logo
                });
            }

            List<ClientStreamsDto> streamDtos = [];

            foreach (KeyValuePair<string, Stream> stream in channelDistributor.ClientStreams)
            {
                SMStream? test = smStreams.FirstOrDefault(a => a.Id.ToString() == stream.Key);
                streamDtos.Add(new ClientStreamsDto()
                {
                    SMStreamId = stream.Key,
                    Name = stream.Key,
                    Logo = test?.Logo
                });
            }

            ChannelDistributorDto dto = new()
            {
                Name = channelDistributor.Name,
                //SMStreamInfo = channelDistributor.SMStreamInfo,
                ClientChannels = channelDtos,
                ClientStreams = streamDtos,
                GetChannelItemCount = channelDistributor.GetChannelItemCount,
                GetMetrics = channelDistributor.GetMetrics,
                IsFailed = channelDistributor.IsFailed
            };

            foreach (string channel in channelDistributor.ClientChannels.Keys)
            {

            }

            dtos.Add(dto);
        }


        return DataResponse<List<ChannelDistributorDto>>.Success(dtos);
    }
}
