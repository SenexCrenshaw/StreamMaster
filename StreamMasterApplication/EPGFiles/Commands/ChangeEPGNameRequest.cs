using AutoMapper;

using FluentValidation;

using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.EPGFiles.Commands;

public class ChangeEPGFileNameRequest : IRequest<EPGFilesDto?>
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
}

public class ChangeEPGFileNameRequestValidator : AbstractValidator<ChangeEPGFileNameRequest>
{
    public ChangeEPGFileNameRequestValidator()
    {
        _ = RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(32)
            .NotEmpty();
    }
}

public class ChangeEPGFileNameRequestHandler : IRequestHandler<ChangeEPGFileNameRequest, EPGFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public ChangeEPGFileNameRequestHandler(
          IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<EPGFilesDto?> Handle(ChangeEPGFileNameRequest request, CancellationToken cancellationToken)
    {
        EPGFile? epgFile = await _context.EPGFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        epgFile.Name = request.Name;
        EPGFilesDto ret = _mapper.Map<EPGFilesDto>(epgFile);

        epgFile.AddDomainEvent(new EPGFileChangedEvent(ret));
        _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
