using AutoMapper;

using MediatR;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFile(int Id) : IRequest<M3UFilesDto?>;

internal class GetM3UFileHandler : IRequestHandler<GetM3UFile, M3UFilesDto?>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;

    public GetM3UFileHandler(
         IMapper mapper,
        IAppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<M3UFilesDto?> Handle(GetM3UFile request, CancellationToken cancellationToken = default)
    {
        if ( request.Id == 0 ) return new M3UFilesDto { Id = 0, Name = "All" };

        M3UFile? m3uFile = await _context.M3UFiles.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        return m3uFile == null ? null : _mapper.Map<M3UFilesDto>(m3uFile);
    }
}
