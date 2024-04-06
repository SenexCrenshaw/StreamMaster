using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles;

public interface IM3UFileTasks
{
    ValueTask ProcessM3UFile(ProcessM3UFileRequest pr, bool immediate = false, CancellationToken cancellationToken = default);

    ValueTask ProcessM3UFiles(CancellationToken cancellationToken = default);

    ValueTask ScanDirectoryForM3UFiles(CancellationToken cancellationToken = default);
}
