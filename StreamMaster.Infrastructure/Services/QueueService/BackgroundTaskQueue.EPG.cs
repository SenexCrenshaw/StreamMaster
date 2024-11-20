using StreamMaster.Application.EPGFiles;
using StreamMaster.Domain.Enums;

namespace StreamMaster.Infrastructure.Services.QueueService;

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

    public async ValueTask EPGSync(CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.EPGSync, cancellationToken).ConfigureAwait(false);
    }
}
