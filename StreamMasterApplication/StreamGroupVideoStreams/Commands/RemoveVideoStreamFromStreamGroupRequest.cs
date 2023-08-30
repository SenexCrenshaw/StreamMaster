using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroupVideoStreams.Commands;

[RequireAll]
public record RemoveVideoStreamFromStreamGroupRequest(int StreamGroupId, string VideoStreamId) : IRequest { }

public class RemoveVideoStreamFromStreamGroupRequestValidator : AbstractValidator<RemoveVideoStreamFromStreamGroupRequest>
{
    public RemoveVideoStreamFromStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class RemoveVideoStreamFromStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<RemoveVideoStreamFromStreamGroupRequest>
{
    public RemoveVideoStreamFromStreamGroupRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return;
        }

        await Repository.StreamGroupVideoStream.RemoveVideoStreamFromStreamGroup(request.StreamGroupId, request.VideoStreamId, cancellationToken).ConfigureAwait(false);
      
    }
}
