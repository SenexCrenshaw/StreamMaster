﻿using FluentValidation;

using MediatR;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public record DeleteVideoStreamRequest(int VideoStreamId) : IRequest<int?>
{
}

public class DeleteVideoStreamRequestValidator : AbstractValidator<DeleteVideoStreamRequest>
{
    public DeleteVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamId).NotNull().GreaterThan(0);
    }
}

public class DeleteVideoStreamRequestHandler : IRequestHandler<DeleteVideoStreamRequest, int?>
{
    private readonly IAppDbContext _context;

    private readonly IPublisher _publisher;

    public DeleteVideoStreamRequestHandler(

         IPublisher publisher,
        IAppDbContext context
        )
    {
        _publisher = publisher;

        _context = context;
    }

    public async Task<int?> Handle(DeleteVideoStreamRequest request, CancellationToken cancellationToken)
    {
        if (await _context.DeleteVideoStreamAsync(request.VideoStreamId, cancellationToken))
        {
            await _publisher.Publish(new DeleteVideoStreamEvent(request.VideoStreamId), cancellationToken).ConfigureAwait(false);
            return request.VideoStreamId;
        }

        return null;
    }
}
