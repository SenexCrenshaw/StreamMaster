namespace StreamMaster.Application.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler(ILogger<GetStreamGroup> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {


        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.Id).ConfigureAwait(false);
        return streamGroup;
    }
}
