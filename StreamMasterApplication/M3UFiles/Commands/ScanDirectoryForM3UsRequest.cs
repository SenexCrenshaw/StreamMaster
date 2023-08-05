using MediatR;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ScanDirectoryForM3UFilesRequest : IRequest<bool>
{
}

public class ScanDirectoryForM3UFilesRequestHandler : IRequestHandler<ScanDirectoryForM3UFilesRequest, bool>
{
    private IRepositoryWrapper Repository { get; }
    public ScanDirectoryForM3UFilesRequestHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<bool> Handle(ScanDirectoryForM3UFilesRequest command, CancellationToken cancellationToken)
    {
        var fd = FileDefinitions.M3U;

        DirectoryInfo m3uDirInfo = new(fd.DirectoryLocation);

        EnumerationOptions er = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive
        };

        foreach (FileInfo m3uFileInfo in m3uDirInfo.GetFiles($"*{fd.FileExtension}", er))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (m3uFileInfo.DirectoryName == null)
            {
                continue;
            }

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileBySourceAsync(m3uFileInfo.Name).ConfigureAwait(false);
            if (m3uFile == null)
            {
                string Url = "";
                string filePath = Path.Combine(m3uFileInfo.DirectoryName, Path.GetFileNameWithoutExtension(m3uFileInfo.FullName) + ".url");

                if (FileUtil.ReadUrlFromFile(filePath, out string? url))
                {
                    if (url is not null)
                    {
                        Url = url;
                    }
                }

                m3uFile = new M3UFile
                {
                    Name = Path.GetFileNameWithoutExtension(m3uFileInfo.Name),
                    Source = m3uFileInfo.Name,
                    Description = $"Imported from {m3uFileInfo.Name}",
                    LastDownloaded = m3uFileInfo.LastWriteTime,
                    LastDownloadAttempt = DateTime.Now,
                    FileExists = true,
                    MaxStreamCount = 1,
                    StartingChannelNumber = 1,
                    Url = Url
                };
                m3uFile.SetFileDefinition(FileDefinitions.M3U);

                Repository.M3UFile.CreateM3UFile(m3uFile);
                await Repository.SaveAsync().ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(m3uFile.Url))
            {
                m3uFile.LastDownloaded = m3uFileInfo.LastWriteTime;
                await Repository.SaveAsync().ConfigureAwait(false);
            }

            //M3UFileDto ret = _mapper.Map<M3UFileDto>(m3uFile);
            //await _publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return true;
    }
}
