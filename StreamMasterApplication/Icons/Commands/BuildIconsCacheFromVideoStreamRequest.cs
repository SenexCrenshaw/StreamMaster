using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace StreamMasterApplication.Icons.Commands;

public class BuildIconsCacheFromVideoStreamRequest : IRequest<bool>
{
}

public class BuildIconsCacheFromVideoStreamRequestHandler : IRequestHandler<BuildIconsCacheFromVideoStreamRequest, bool>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public BuildIconsCacheFromVideoStreamRequestHandler(
           IMemoryCache memoryCache,
           IMapper mapper,
         IAppDbContext context)
    {
        _mapper = mapper;
        _memoryCache = memoryCache;
        _context = context;
    }

    public async Task<bool> Handle(BuildIconsCacheFromVideoStreamRequest command, CancellationToken cancellationToken)
    {
        return await _context.BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false);
    }
}
