using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.StreamGroups.Queries;

public record GetStreamGroup(int Id) : IRequest<StreamGroupDto?>;

internal class GetStreamGroupHandler(ILogger<GetStreamGroup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroup, StreamGroupDto?>
{
    public async Task<StreamGroupDto?> Handle(GetStreamGroup request, CancellationToken cancellationToken = default)
    {
        if (request.Id == 0)
        {
            return new StreamGroupDto { Id = 0, Name = "All" };
        }


        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupById(request.Id).ConfigureAwait(false);
        return streamGroup;
    }
}
