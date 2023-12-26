using MediatR;

using StreamMaster.Domain.Dto;

namespace StreamMaster.Domain.Requests;

public class CreateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?> { }
