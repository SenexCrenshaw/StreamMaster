using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons.Queries;

public record GetIcons : IRequest<List<IconFileDto>>;

internal class GetIconsQueryHandler : IRequestHandler<GetIcons, List<IconFileDto>>
{
    private readonly IAppDbContext _context;

    public GetIconsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        return await _context.GetIcons(cancellationToken);
    }
}
