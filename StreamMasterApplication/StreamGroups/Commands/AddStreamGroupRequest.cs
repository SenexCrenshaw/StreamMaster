using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.StreamGroups.Commands;


public class AddStreamGroupRequestValidator : AbstractValidator<AddStreamGroupRequest>
{
    public AddStreamGroupRequestValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotEmpty()
            .GreaterThan(0);

        _ = RuleFor(v => v.Name)
           .MaximumLength(32)
           .NotEmpty();
    }
}

public class AddStreamGroupRequestHandler : BaseMediatorRequestHandler, IRequestHandler<AddStreamGroupRequest>
{
    protected Setting _setting = FileUtil.GetSetting();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddStreamGroupRequestHandler(IHttpContextAccessor httpContextAccessor, ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Handle(AddStreamGroupRequest command, CancellationToken cancellationToken)
    {
        if (command.StreamGroupNumber < 0)
        {
            return;
        }

        _ = Repository.StreamGroup.AddStreamGroupRequestAsync(command, cancellationToken);

        await Publisher.Publish(new StreamGroupUpdateEvent(), cancellationToken).ConfigureAwait(false);

    }
}
