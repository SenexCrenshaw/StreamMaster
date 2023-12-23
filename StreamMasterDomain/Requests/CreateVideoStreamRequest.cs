using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Requests;

public class CreateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?> { }
