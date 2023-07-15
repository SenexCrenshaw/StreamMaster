using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Icons.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Icons;

public interface IIconController
{
    Task<ActionResult> AddIconFile(AddIconFileRequest request);

    Task<ActionResult> AddIconFileFromForm([FromForm] AddIconFileRequest request);

    Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<ActionResult<IconFileDto>> GetIcon(int Id);

    Task<ActionResult<IEnumerable<IconFileDto>>> GetIcons();
}

public interface IIconDB
{
    DbSet<IconFile> Icons { get; set; }

    Task<List<IconFileDto>> GetIcons(CancellationToken cancellationToken);
}

public interface IIconHub
{
    Task<IconFileDto?> AddIconFile(AddIconFileRequest request);

    Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<IconFileDto?> GetIcon(int Id);

    Task<IEnumerable<IconFileDto>> GetIcons();
}

public interface IIconScoped
{
}

public interface IIconTasks
{
    ValueTask CacheAllIcons(CancellationToken cancellationToken = default);

    ValueTask CacheIconsFromEPGs(CancellationToken cancellationToken = default);

    ValueTask CacheIconsFromVideoStreams(CancellationToken cancellationToken = default);

    Task ReadDirectoryLogosRequest(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default);
}
