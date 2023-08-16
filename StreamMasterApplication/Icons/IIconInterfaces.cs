using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Icons;

public interface IIconController
{
    Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<ActionResult<IconFileDto>> GetIcon(int Id);
    Task<ActionResult<IconFileDto>> GetIconFromSource(string source);
    Task<ActionResult<PagedResponse<IconFileDto>>> GetIcons(IconFileParameters iconFileParameters);
    Task<ActionResult<IEnumerable<IconFileDto>>> GetIconsSimpleQuery(IconFileParameters iconFileParameters);
}

public interface IIconDB
{
}

public interface IIconHub
{
    Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);
    Task<IconFileDto?> GetIconFromSource(string source);
    Task<IconFileDto?> GetIcon(int Id);
    Task<PagedResponse<IconFileDto>> GetIcons(IconFileParameters iconFileParameters);
    Task<IEnumerable<IconFileDto>> GetIconsSimpleQuery(IconFileParameters iconFileParameters);
}

public interface IIconScoped
{
}

public interface IIconTasks
{
    ValueTask BuildIconCaches(CancellationToken cancellationToken = default);

    ValueTask BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken = default);

    ValueTask BuildProgIconsCacheFromEPGs(CancellationToken cancellationToken = default);

    Task ReadDirectoryLogosRequest(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default);
}