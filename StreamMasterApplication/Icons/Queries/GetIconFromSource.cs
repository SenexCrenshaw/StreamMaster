using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

using System.Web;

namespace StreamMasterApplication.Icons.Queries;

public record GetIconFromSource(string source) : IRequest<IconFileDto?>;

internal class GetIconFromSourceHandler : IRequestHandler<GetIconFromSource, IconFileDto?>
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IMemoryCache _memoryCache;
    public GetIconFromSourceHandler(
        IMemoryCache memoryCache,
            ISender sender,
    IMapper mapper
        )
    {
        _memoryCache = memoryCache;
        _sender = sender;
        _mapper = mapper;
    }

    public Task<IconFileDto?> Handle(GetIconFromSource request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.source))
        {
            return Task.FromResult<IconFileDto?>(null);
        }

        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
        string toCheck = HttpUtility.UrlDecode(request.source).ToLower();

        IconFileDto? icon = icons.FirstOrDefault(a => a.Source.ToLower() == toCheck);
        return Task.FromResult(icon);
    }
}