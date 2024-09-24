using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Logos.CommandsOld;

namespace StreamMaster.Application.Logos;

public interface ILogoController
{

    Task<ActionResult> AutoMatchLogoToStreams(AutoMatchLogoToStreamsRequest request);

}

public interface ILogoDB
{
}

public interface ILogoHub
{
    Task AutoMatchLogoToStreams(AutoMatchLogoToStreamsRequest request);

}

public interface ILogoScoped { }

public interface ILogoTasks
{
    ValueTask BuildLogoCaches(CancellationToken cancellationToken = default);

    ValueTask BuildLogosCacheFromVideoStreams(CancellationToken cancellationToken = default);

    ValueTask BuildProgLogosCacheFromEPGs(CancellationToken cancellationToken = default);

    Task ReadDirectoryLogos(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForLogoFiles(CancellationToken cancellationToken = default);
}