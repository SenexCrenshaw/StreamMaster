using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetAllStatisticsForAllUrls() : IRequest<List<StreamStatisticsResult>>;

internal class GetAllStatisticsForAllUrlsHandler(IChannelManager channelManager, ISettingsService settingsService) : IRequestHandler<GetAllStatisticsForAllUrls, List<StreamStatisticsResult>>
{
    public async Task<List<StreamStatisticsResult>> Handle(GetAllStatisticsForAllUrls request, CancellationToken cancellationToken)
    {

        List<StreamStatisticsResult> ret = await channelManager.GetAllStatisticsForAllUrls();

        return ret;
    }
}
