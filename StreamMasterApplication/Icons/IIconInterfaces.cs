using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Icons.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons;

public interface IIconController
{
    Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<ActionResult<IconFileDto>> GetIcon(int Id);

    Task<ActionResult<IEnumerable<IconFileDto>>> GetIcons();
}

public interface IIconDB
{
}

public interface IIconHub
{
    Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<IconFileDto?> GetIcon(int Id);

    Task<IEnumerable<IconFileDto>> GetIcons();
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
