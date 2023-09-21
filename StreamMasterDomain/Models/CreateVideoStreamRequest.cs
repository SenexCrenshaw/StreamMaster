using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Models;

public class CreateVideoStreamRequest : VideoStreamBaseRequest, IRequest<VideoStreamDto?> { }
