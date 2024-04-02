﻿using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Icons.CommandsOld;
using StreamMaster.Application.Icons.Queries;

namespace StreamMaster.Application.Icons;

public interface IIconController
{

    Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

    Task<ActionResult<IconFileDto>> GetIconFromSource(GetIconFromSourceRequest request);
    Task<ActionResult<PagedResponse<IconFileDto>>> GetPagedIcons(IconFileParameters iconFileParameters);
    Task<ActionResult<IEnumerable<IconFileDto>>> GetIconsSimpleQuery(IconFileParameters iconFileParameters);
}

public interface IIconDB
{
}

public interface IIconHub
{
    Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);
    Task<IconFileDto?> GetIconFromSource(GetIconFromSourceRequest request);

    Task<PagedResponse<IconFileDto>> GetPagedIcons(IconFileParameters iconFileParameters);
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

    Task ReadDirectoryLogos(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default);
}