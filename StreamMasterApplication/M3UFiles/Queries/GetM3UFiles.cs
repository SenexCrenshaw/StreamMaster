using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFiles : IRequest<IEnumerable<M3UFilesDto>>;

internal class GetM3UFilesHandler : IRequestHandler<GetM3UFiles, IEnumerable<M3UFilesDto>>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetM3UFilesHandler(
        IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<IEnumerable<M3UFilesDto>> Handle(GetM3UFiles request, CancellationToken cancellationToken = default)
    {
        List<M3UFilesDto> ret = await _context.M3UFiles
           .AsNoTracking()
           .ProjectTo<M3UFilesDto>(_mapper.ConfigurationProvider)
           .OrderBy(x => x.Name)
           .ToListAsync(cancellationToken).ConfigureAwait(false);

        ret.Insert  (0, new M3UFilesDto { Id = 0, Name = "All" });

        return ret;
    }
}
