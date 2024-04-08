namespace StreamMaster.Application.StreamGroups.QueriesOld;

public record GetStreamGroup(int Id) : IRequest<DataResponse<StreamGroupDto?>>;

internal class GetStreamGroupHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroup, DataResponse<StreamGroupDto?>>
{
    public async Task<DataResponse<StreamGroupDto?>> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.Id).ConfigureAwait(false);
        return DataResponse<StreamGroupDto?>.Success(streamGroup);
    }
}
