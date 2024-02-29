namespace StreamMaster.Application.Icons.Queries;
public record GetIcons() : IRequest<List<IconFileDto>>;

internal class GetIconsHandler(ILogger<GetIcons> logger, IIconService iconService)
    : IRequestHandler<GetIcons, List<IconFileDto>>
{
    public Task<List<IconFileDto>> Handle(GetIcons request, CancellationToken cancellationToken)
    {
        List<IconFileDto> icons = iconService.GetIcons();

        return Task.FromResult(icons);
    }
}