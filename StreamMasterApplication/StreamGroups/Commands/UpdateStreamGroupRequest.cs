using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Extensions;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups.Commands;

public class UpdateStreamGroupRequestValidator : AbstractValidator<UpdateStreamGroupRequest>
{
    public UpdateStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupId)
           .NotNull()
           .GreaterThanOrEqualTo(0);
    }
}

public class UpdateStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateStreamGroupRequest, StreamGroupDto?>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateStreamGroupRequestHandler(IHttpContextAccessor httpContextAccessor, ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StreamGroupDto?> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return null;
        }
        string url = _httpContextAccessor.GetUrl();
        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroupAsync(request, url, cancellationToken).ConfigureAwait(false);
        if (streamGroup is not null)
        {
            //var streamGroup = await _context.GetStreamGroupDto(ret.Id, url, cancellationToken).ConfigureAwait(false);
            if (streamGroup is not null && streamGroup.ChildVideoStreams.Any())
            {
                await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
            }
            await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);
        }

        return streamGroup;
    }
}
