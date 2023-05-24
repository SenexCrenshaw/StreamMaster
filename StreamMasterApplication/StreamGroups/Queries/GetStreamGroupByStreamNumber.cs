using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

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

internal class GetStreamGroupByStreamNumberHandler : IRequestHandler<GetStreamGroupByStreamNumber, StreamGroupDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetStreamGroupByStreamNumberHandler(
         IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {
        StreamGroup? streamGroup = await _context.StreamGroups
            .Include(a => a.VideoStreams)
            .FirstOrDefaultAsync(a => a.StreamGroupNumber == request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false);
        return streamGroup == null ? null : _mapper.Map<StreamGroupDto>(streamGroup);
    }
}
