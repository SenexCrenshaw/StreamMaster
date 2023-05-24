using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ChangeM3UFileNameRequest : IRequest<M3UFilesDto?>
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

public class ChangeM3UFileNameRequestHandler : IRequestHandler<ChangeM3UFileNameRequest, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public ChangeM3UFileNameRequestHandler(
    IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<M3UFilesDto?> Handle(ChangeM3UFileNameRequest request, CancellationToken cancellationToken)
    {
        M3UFile? m3UFile = await _context.M3UFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }

        m3UFile.Name = request.Name;
        M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3UFile);
        m3UFile.AddDomainEvent(new M3UFileChangedEvent(ret));
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
