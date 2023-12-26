using MediatR;

using StreamMaster.Domain.Dto;

namespace StreamMaster.Domain.Requests;

public record UpdateVideoStreamsRequest(IEnumerable<UpdateVideoStreamRequest> VideoStreamUpdates) : IRequest<List<VideoStreamDto>> { }
