namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamingStatisticsForChannelRequest(int ChannelId) : IRequest<DataResponse<List<StreamStreamingStatistic>>>;

internal class GetStreamingStatisticsForChannelRequestHandler(IStreamStatisticService streamStatisticService, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<GetStreamingStatisticsForChannelRequest, DataResponse<List<StreamStreamingStatistic>>>
{
    public async Task<DataResponse<List<StreamStreamingStatistic>>> Handle(GetStreamingStatisticsForChannelRequest request, CancellationToken cancellationToken)
    {
        var streamingStatistics = streamStatisticService.GetStreamStreamingStatistics();
        var smChannel = repositoryWrapper.SMChannel.GetSMChannel(request.ChannelId);
        if (smChannel == null)
        {
            return DataResponse<List<StreamStreamingStatistic>>.ErrorWithMessage("Channel not found");
        }

        var smStreamIds = smChannel.SMStreams.Select(a => a.SMStreamId).ToList();

        var channelStreamingStatistics = streamingStatistics.Where(a => smStreamIds.Contains(a.Id)).ToList();
        var channelStreamingStatisticsIds = channelStreamingStatistics.Select(a => a.Id).ToList();

        var smStreamsToCreate = smChannel.SMStreams.Where(a => !channelStreamingStatisticsIds.Contains(a.SMStreamId)).ToList();

        foreach (var stat in channelStreamingStatistics)
        {
            stat.UpdateValues();
        }

        foreach (var smStream in smStreamsToCreate)
        {
            channelStreamingStatistics.Add(new StreamStreamingStatistic
            {
                StreamName = smStream.SMStream.Name,
                StreamLogo = smStream.SMStream.Logo,
                StartTime = SMDT.UtcNow,
                StreamUrl = smStream.SMStream.Url,
                Id = smStream.SMStream.Id
            });
        }

        List<SMChannelStreamLink> links = repositoryWrapper.SMChannelStreamLink.GetQuery().Where(a => a.SMChannelId == request.ChannelId).ToList();

        foreach (var stat in channelStreamingStatistics)
        {

            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stat.Id);

            if (link != null)
            {
                stat.Rank = link.Rank;
            }
        }

        return DataResponse<List<StreamStreamingStatistic>>.Success(channelStreamingStatistics.OrderBy(a => a.Rank).ToList());


    }
}
