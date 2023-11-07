using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineupPreviews : IRequest<List<LineupPreview>>;

internal class GetLineupPreviewsHandler(ISDService sdService) : IRequestHandler<GetLineupPreviews, List<LineupPreview>>
{
    public async Task<List<LineupPreview>> Handle(GetLineupPreviews request, CancellationToken cancellationToken)
    {

        List<LineupPreview> ret = await sdService.GetLineupPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
