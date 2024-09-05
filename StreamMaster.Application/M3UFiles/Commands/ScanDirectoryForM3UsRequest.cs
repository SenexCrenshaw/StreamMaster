namespace StreamMaster.Application.M3UFiles.Commands;

public record ScanDirectoryForM3UFilesRequest : IRequest<DataResponse<bool>>;

[LogExecutionTimeAspect]
public class ScanDirectoryForM3UFilesRequestHandler(IPublisher Publisher, ICacheManager CacheManager, IFileUtilService fileUtilService, IRepositoryWrapper Repository, IMapper Mapper)
    : IRequestHandler<ScanDirectoryForM3UFilesRequest, DataResponse<bool>>
{
    public async Task<DataResponse<bool>> Handle(ScanDirectoryForM3UFilesRequest command, CancellationToken cancellationToken)
    {
        IEnumerable<FileInfo> m3uFiles = fileUtilService.GetFilesFromDirectory(FileDefinitions.M3U);
        foreach (FileInfo m3uFileInfo in m3uFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return DataResponse.False;
            }

            await ProcessM3UFile(m3uFileInfo, cancellationToken);
        }

        return DataResponse.True;
    }

    private async Task ProcessM3UFile(FileInfo m3uFileInfo, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(m3uFileInfo.DirectoryName))
        {
            return;
        }

        M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileBySource(m3uFileInfo.Name).ConfigureAwait(false);
        if (m3uFile == null)
        {
            m3uFile = CreateOrUpdateM3UFile(m3uFileInfo);
            await SaveM3UFile(m3uFile);
        }

        if (m3uFile != null)
        {
            CacheManager.M3UMaxStreamCounts.AddOrUpdate(m3uFile.Id, m3uFile.MaxStreamCount, (_, _) => m3uFile.MaxStreamCount);

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            await Publisher.Publish(new M3UFileProcessEvent(ret.Id, false), cancellationToken).ConfigureAwait(false);
        }
    }

    private static M3UFile CreateOrUpdateM3UFile(FileInfo m3uFileInfo)
    {
        M3UFile m3uFile = M3UFile.ReadJSON(m3uFileInfo) ?? new M3UFile
        {
            Name = Path.GetFileNameWithoutExtension(m3uFileInfo.Name),
            Source = m3uFileInfo.Name,
            LastDownloaded = m3uFileInfo.LastWriteTime,
            LastDownloadAttempt = SMDT.UtcNow,
            FileExists = true,
            MaxStreamCount = 1,
            Url = ""
        };

        m3uFile.LastUpdated = DateTime.MinValue;
        m3uFile.SetFileDefinition(FileDefinitions.M3U);

        return m3uFile;
    }

    private async Task SaveM3UFile(M3UFile m3uFile)
    {
        Repository.M3UFile.CreateM3UFile(m3uFile);

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        m3uFile.WriteJSON();

        if (string.IsNullOrEmpty(m3uFile.Url))
        {
            m3uFile.LastDownloaded = SMDT.UtcNow;
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            m3uFile.WriteJSON();
        }
    }
}
