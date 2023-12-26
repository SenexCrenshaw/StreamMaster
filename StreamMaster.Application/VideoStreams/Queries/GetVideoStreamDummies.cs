using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.VideoStreams.Queries;

public record GetVideoStreamDummies() : IRequest<List<VideoStreamDto>>;

internal class GetVideoStreamDummiesHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetVideoStreamDummies, List<VideoStreamDto>>
{
    public Task<List<VideoStreamDto>> Handle(GetVideoStreamDummies request, CancellationToken cancellationToken)
    {
        List<VideoStream> dummies = [.. Repository.VideoStream.FindByCondition(x => x.User_Tvg_ID == "DUMMY")];
        return Task.FromResult(Mapper.Map<List<VideoStreamDto>>(dummies));
    }
}
