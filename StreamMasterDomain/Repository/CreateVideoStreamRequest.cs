using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Repository;

public class CreateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?> { }
