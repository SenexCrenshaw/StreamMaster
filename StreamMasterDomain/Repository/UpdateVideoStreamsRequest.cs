using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Repository;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<List<VideoStreamDto>> { }
