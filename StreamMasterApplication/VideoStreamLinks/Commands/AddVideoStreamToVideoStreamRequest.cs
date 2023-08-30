using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.VideoStreamLinks.Commands;

[RequireAll]
public record AddVideoStreamToVideoStreamRequest(string ParentVideoStreamId, string ChildVideoStreamId, int? Rank) : IRequest { }

public class AddVideoStreamToVideoStreamRequestValidator : AbstractValidator<AddVideoStreamToVideoStreamRequest>
{
    public AddVideoStreamToVideoStreamRequestValidator()
    {
    }
}

public class AddVideoStreamToVideoStreamRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddVideoStreamToVideoStreamRequest>
{
    public AddVideoStreamToVideoStreamRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task Handle(AddVideoStreamToVideoStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.VideoStreamLink.AddVideoStreamTodVideoStream(request.ParentVideoStreamId, request.ChildVideoStreamId, request.Rank, cancellationToken).ConfigureAwait(false);
        
    }
}