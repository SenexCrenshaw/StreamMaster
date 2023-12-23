using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Requests;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<List<VideoStreamDto>> { }
