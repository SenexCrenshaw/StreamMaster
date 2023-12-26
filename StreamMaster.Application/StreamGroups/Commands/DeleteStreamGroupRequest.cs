using FluentValidation;

using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

public record DeleteStreamGroupRequest(int Id) : IRequest<int?> { }

public class DeleteStreamGroupRequestValidator : AbstractValidator<DeleteStreamGroupRequest>
{
    public DeleteStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.Id)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

public class DeleteStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<DeleteStreamGroupRequest, int?>
{

    public DeleteStreamGroupRequestHandler(ILogger<DeleteStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
  : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<int?> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id < 1)
        {
            return null;
        }

        if (await Repository.StreamGroup.DeleteStreamGroup(request.Id) != null)
        {
            await Repository.SaveAsync();
            await Publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken);
            return request.Id;
        }

        return null;
    }
}
