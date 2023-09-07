using FluentValidation;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroupByStreamNumber(int StreamGroupNumber) : IRequest<StreamGroupDto?>;

public class GetStreamGroupByStreamNumberValidator : AbstractValidator<GetStreamGroupByStreamNumber>
{
    public GetStreamGroupByStreamNumberValidator()
    {
        _ = RuleFor(v => v.StreamGroupNumber)
            .NotNull().GreaterThanOrEqualTo(0);
    }
}

internal class GetStreamGroupByStreamNumberHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroupByStreamNumber, StreamGroupDto?>
{


    public GetStreamGroupByStreamNumberHandler(ILogger<GetStreamGroupByStreamNumber> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper, publisher, sender, hubContext) { }



    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {

        StreamGroupDto? streamGroup = await Repository.StreamGroup.GetStreamGroupDtoByStreamGroupNumber(request.StreamGroupNumber, cancellationToken).ConfigureAwait(false);

        return streamGroup;
    }
}
