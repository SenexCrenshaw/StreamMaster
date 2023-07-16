using MediatR;

using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetAllStatisticsForAllUrls() : IRequest<List<StreamStatisticsResult>>;

internal class GetAllStatisticsForAllUrlsHandler : IRequestHandler<GetAllStatisticsForAllUrls, List<StreamStatisticsResult>>
{
    private readonly IChannelManager _channelManager;

    public GetAllStatisticsForAllUrlsHandler(IChannelManager channelManager)
    {
        _channelManager = channelManager;
    }

    public Task<List<StreamStatisticsResult>> Handle(GetAllStatisticsForAllUrls request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_channelManager.GetAllStatisticsForAllUrls());
    }
}
