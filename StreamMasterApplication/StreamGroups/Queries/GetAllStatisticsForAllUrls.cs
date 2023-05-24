using MediatR;

using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetAllStatisticsForAllUrls() : IRequest<List<StreamStatisticsResult>>;

internal class GetAllStatisticsForAllUrlsHandler : IRequestHandler<GetAllStatisticsForAllUrls, List<StreamStatisticsResult>>
{
    private readonly IRingBufferManager _ringBufferManager;

    public GetAllStatisticsForAllUrlsHandler(IRingBufferManager ringBufferManager)
    {
        _ringBufferManager = ringBufferManager;
    }

    public Task<List<StreamStatisticsResult>> Handle(GetAllStatisticsForAllUrls request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_ringBufferManager.GetAllStatisticsForAllUrls());
    }
}
