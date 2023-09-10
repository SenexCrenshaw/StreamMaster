using MediatR;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineupPreviews : IRequest<List<LineUpPreview>>;

internal class GetLineupPreviewsHandler : IRequestHandler<GetLineupPreviews, List<LineUpPreview>>
{
    public async Task<List<LineUpPreview>> Handle(GetLineupPreviews request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();

        var ret = await sd.GetLineUpPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
