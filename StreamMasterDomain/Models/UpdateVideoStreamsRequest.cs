using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Models;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<List<VideoStreamDto>> { }
