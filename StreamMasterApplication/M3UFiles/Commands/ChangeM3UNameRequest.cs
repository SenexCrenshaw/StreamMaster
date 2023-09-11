using FluentValidation;

namespace StreamMasterApplication.M3UFiles.Commands;

public record ChangeM3UFileNameRequest(int Id, string Name) : IRequest<bool> { }

public class ChangeM3UFileNameRequestValidator : AbstractValidator<ChangeM3UFileNameRequest>
{
    public ChangeM3UFileNameRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(32)
            .NotEmpty();
    }
}

public class ChangeM3UFileNameRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ChangeM3UFileNameRequest, bool>
{
    public ChangeM3UFileNameRequestHandler(ILogger<ChangeM3UFileNameRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
  : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }

    public async Task<bool> Handle(ChangeM3UFileNameRequest request, CancellationToken cancellationToken)
    {
        M3UFile m3UFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return false;
        }

        m3UFile.Name = request.Name;

        Repository.M3UFile.UpdateM3UFile(m3UFile);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        M3UFileDto ret = Mapper.Map<M3UFileDto>(m3UFile);

        await Publisher.Publish(new M3UFileChangedEvent(ret));

        return true;
    }
}
