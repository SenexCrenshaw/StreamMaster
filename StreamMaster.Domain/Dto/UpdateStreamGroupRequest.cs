﻿using MediatR;

namespace StreamMaster.Domain.Dto;
public record UpdateStreamGroupRequest(int StreamGroupId, string? Name, bool? AutoSetChannelNumbers, string? FFMPEGProfileId) : IRequest<StreamGroupDto?> { }
