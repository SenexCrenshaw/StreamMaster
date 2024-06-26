using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Icons.CommandsOld;

namespace StreamMaster.Application.Icons;

public interface IIconController
{

    Task<ActionResult> AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

}

public interface IIconDB
{
}

public interface IIconHub
{
    Task AutoMatchIconToStreams(AutoMatchIconToStreamsRequest request);

}

public interface IIconScoped { }

public interface IIconTasks
{
    ValueTask BuildIconCaches(CancellationToken cancellationToken = default);

    ValueTask BuildIconsCacheFromVideoStreams(CancellationToken cancellationToken = default);

    ValueTask BuildProgIconsCacheFromEPGs(CancellationToken cancellationToken = default);

    Task ReadDirectoryLogos(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForIconFiles(CancellationToken cancellationToken = default);
}