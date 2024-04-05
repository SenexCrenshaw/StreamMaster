namespace StreamMaster.Application.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<APIResponse<StreamGroupDto?>>;

internal class GetStreamGroupHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroup, APIResponse<StreamGroupDto?>>
{
    public async Task<APIResponse<StreamGroupDto?>> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.Id).ConfigureAwait(false);
        return APIResponse<StreamGroupDto?>.Success(streamGroup);
    }
}
