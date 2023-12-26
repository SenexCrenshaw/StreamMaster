using StreamMaster.Domain.Enums;

using StreamMaster.Application.EPGFiles;

namespace StreamMaster.Infrastructure.Services;

public partial class BackgroundTaskQueue : IEPGFileTasks
{
    public async ValueTask ProcessEPGFile(int EPGFileId, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ProcessEPGFile, EPGFileId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ScanDirectoryForEPGFiles(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.ScanDirectoryForEPGFiles, cancellationToken).ConfigureAwait(false);
    }
}
