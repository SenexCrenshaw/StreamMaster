using AutoMapper;
using AutoMapper.QueryableExtensions;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Extensions;

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
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetStreamGroupByStreamNumberHandler(
         IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IAppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<StreamGroupDto?> Handle(GetStreamGroupByStreamNumber request, CancellationToken cancellationToken = default)
    {
        var streamGroup = await _context.StreamGroups.FirstOrDefaultAsync(a=>a.StreamGroupNumber== request.StreamGroupNumber).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return null;
        }

        var url = _httpContextAccessor.GetUrl();
        var ret = await _context.GetStreamGroupDto(streamGroup.Id, url, cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
