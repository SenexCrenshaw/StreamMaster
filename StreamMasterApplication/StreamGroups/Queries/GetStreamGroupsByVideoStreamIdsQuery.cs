using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupsByVideoStreamIdsQuery(List<string> VideoStreamIds, string Url) : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsByVideoStreamIdsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupsByVideoStreamIdsQuery, IEnumerable<StreamGroupDto>>
{
    protected Setting _setting = FileUtil.GetSetting();


    public GetStreamGroupsByVideoStreamIdsQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsByVideoStreamIdsQuery request, CancellationToken cancellationToken = default)
    {

        List<StreamGroupDto> sgs = await Repository.StreamGroup.GetStreamGroupDtos(request.Url, cancellationToken);

        List<StreamGroupDto> matchingStreamGroups = sgs
            .Where(sg => sg.ChildVideoStreams.Any(sgvs => request.VideoStreamIds.Contains(sgvs.Id)))
            .ToList();

        return matchingStreamGroups;

    }
}
