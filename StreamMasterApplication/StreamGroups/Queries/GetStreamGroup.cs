namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler(ILogger<GetStreamGroup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext), IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0)
        {
            return new StreamGroupDto { Id = 0, Name = "All" };
        }


        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDto(request.Id, cancellationToken).ConfigureAwait(false);
        return streamGroup;
    }
}
