using System.Web;

namespace StreamMasterApplication.Icons.Queries;

public record GetIconFromSourceRequest(string value) : IRequest<IconFileDto>;

internal class GetIconFromSourceRequestHandler : IRequestHandler<GetIconFromSourceRequest, IconFileDto>
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IMemoryCache _memoryCache;
    public GetIconFromSourceRequestHandler(
        IMemoryCache memoryCache,
            ISender sender,
    IMapper mapper
        )
    {
        _memoryCache = memoryCache;
        _sender = sender;
        _mapper = mapper;
    }

    public Task<IconFileDto> Handle(GetIconFromSourceRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.value))
        {
            return Task.FromResult(new IconFileDto());
        }

        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);
        string toCheck = HttpUtility.UrlDecode(request.value).ToLower();

        IconFileDto? icon = icons.FirstOrDefault(a => a.Source.ToLower() == toCheck);
        icon ??= new IconFileDto()
        {
            Name = "Icon",
            Source = toCheck,
            Id = 0,
        };

        return Task.FromResult(icon);
    }
}