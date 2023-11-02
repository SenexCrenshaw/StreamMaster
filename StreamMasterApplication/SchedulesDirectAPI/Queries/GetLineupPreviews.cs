using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineupPreviews : IRequest<List<LineUpPreview>>;

internal class GetLineupPreviewsHandler(ISDService sdService) : IRequestHandler<GetLineupPreviews, List<LineUpPreview>>
{
    public async Task<List<LineUpPreview>> Handle(GetLineupPreviews request, CancellationToken cancellationToken)
    {

        List<LineUpPreview> ret = await sdService.GetLineUpPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
