namespace StreamMaster.Application.Statistics.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetChannelStreamingStatisticsRequest() : IRequest<DataResponse<List<ChannelStreamingStatistics>>>;

internal class GetChannelStreamingStatisticHandler(IStreamStatisticService streamStatisticService, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<GetChannelStreamingStatisticsRequest, DataResponse<List<ChannelStreamingStatistics>>>
{
    public async Task<DataResponse<List<ChannelStreamingStatistics>>> Handle(GetChannelStreamingStatisticsRequest request, CancellationToken cancellationToken)
    {
        var channelStreamingStatistics = streamStatisticService.GetChannelStreamingStatistics();
        var streamingStatistics = streamStatisticService.GetStreamStreamingStatistics();

        foreach (var channelStreamingStatistic in channelStreamingStatistics)
        {
            channelStreamingStatistic.UpdateValues();
            channelStreamingStatistic.StreamStreamingStatistics = GetStreamsForChannel(channelStreamingStatistic.Id);

            var streamStat = streamingStatistics.FirstOrDefault(x => x.Rank == channelStreamingStatistic.CurrentRank && x.Id == channelStreamingStatistic.CurrentStreamId);
            if (streamStat is not null)
            {
                channelStreamingStatistic.UpdateStatistic(streamStat);
            }
            else
            {

            }
        }


        return DataResponse<List<ChannelStreamingStatistics>>.Success(channelStreamingStatistics);
    }

    public List<StreamStreamingStatistic> GetStreamsForChannel(int ChannelId)
    {
        var streamingStatistics = streamStatisticService.GetStreamStreamingStatistics();
        var smChannel = repositoryWrapper.SMChannel.GetSMChannel(ChannelId);
        if (smChannel == null)
        {
            return new();
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

        List<SMChannelStreamLink> links = repositoryWrapper.SMChannelStreamLink.GetQuery().Where(a => a.SMChannelId == ChannelId).ToList();

        foreach (var stat in channelStreamingStatistics)
        {

            SMChannelStreamLink? link = links.FirstOrDefault(a => a.SMStreamId == stat.Id);

            if (link != null)
            {
                stat.Rank = link.Rank;
            }
        }

        return channelStreamingStatistics.OrderBy(a => a.Rank).ToList();


    }
}
