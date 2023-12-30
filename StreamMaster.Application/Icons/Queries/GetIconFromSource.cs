using System.Web;

namespace StreamMaster.Application.Icons.Queries;

public record GetIconFromSourceRequest(string value) : IRequest<IconFileDto>;

internal class GetIconFromSourceRequestHandler(IIconService iconService) : IRequestHandler<GetIconFromSourceRequest, IconFileDto>
{
    public Task<IconFileDto> Handle(GetIconFromSourceRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.value))
        {
            return Task.FromResult(new IconFileDto());
        }

        string toCheck = HttpUtility.UrlDecode(request.value);

        IconFileDto? icon = iconService.GetIconBySource(toCheck);
        icon ??= new IconFileDto()
        {
            Name = "Icon",
            Source = toCheck,
        };

        return Task.FromResult(icon);
    }
}