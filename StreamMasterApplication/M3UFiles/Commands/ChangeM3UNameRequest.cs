using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ChangeM3UFileNameRequest : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
}

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

public class ChangeM3UFileNameRequestHandler : BaseRequestHandler, IRequestHandler<ChangeM3UFileNameRequest, bool>
{
    public ChangeM3UFileNameRequestHandler(ILogger<ChangeM3UFileNameRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper)
        : base(logger, repository, mapper) { }

    public async Task<bool> Handle(ChangeM3UFileNameRequest request, CancellationToken cancellationToken)
    {
        var m3UFile = await Repository.M3UFile.GetM3UFileByIdAsync(request.Id).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return false;
        }

        m3UFile.Name = request.Name;

        Repository.M3UFile.UpdateM3UFile(m3UFile);
        await Repository.SaveAsync().ConfigureAwait(false);

        var ret = Mapper.Map<M3UFileDto>(m3UFile);
        m3UFile.AddDomainEvent(new M3UFileChangedEvent(ret));

        return true;
    }
}
