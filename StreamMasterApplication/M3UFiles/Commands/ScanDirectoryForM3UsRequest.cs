using StreamMasterDomain.Models;

namespace StreamMasterApplication.M3UFiles.Commands;

public record ScanDirectoryForM3UFilesRequest : IRequest<bool> { }


[LogExecutionTimeAspect]
public class ScanDirectoryForM3UFilesRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ScanDirectoryForM3UFilesRequest, bool>
{

    public ScanDirectoryForM3UFilesRequestHandler(ILogger<ScanDirectoryForM3UFilesRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<bool> Handle(ScanDirectoryForM3UFilesRequest command, CancellationToken cancellationToken)
    {
        IEnumerable<FileInfo> m3uFiles = GetM3UFilesFromDirectory();
        foreach (FileInfo m3uFileInfo in m3uFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            await ProcessM3UFile(m3uFileInfo, cancellationToken);
        }

        return true;
    }

    private static IEnumerable<FileInfo> GetM3UFilesFromDirectory()
    {
        FileDefinition fd = FileDefinitions.M3U;
        DirectoryInfo m3uDirInfo = new(fd.DirectoryLocation);
        EnumerationOptions er = new() { MatchCasing = MatchCasing.CaseInsensitive };

        return m3uDirInfo.GetFiles($"*{fd.FileExtension}", er);
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
            await SaveM3UFile(m3uFile, cancellationToken);
        }

        if (m3uFile != null)
        {

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            await Publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
        }
    }

    private static M3UFile CreateOrUpdateM3UFile(FileInfo m3uFileInfo)
    {
        M3UFile m3uFile = M3UFile.ReadJSON(m3uFileInfo) ?? new M3UFile
        {
            Name = Path.GetFileNameWithoutExtension(m3uFileInfo.Name),
            Source = m3uFileInfo.Name,
            Description = $"Imported from {m3uFileInfo.Name}",
            LastDownloaded = m3uFileInfo.LastWriteTime,
            LastDownloadAttempt = DateTime.Now,
            FileExists = true,
            MaxStreamCount = 1,
            StartingChannelNumber = 1,
            Url = ""
        };

        m3uFile.LastUpdated = DateTime.MinValue;
        m3uFile.SetFileDefinition(FileDefinitions.M3U);

        return m3uFile;
    }

    private async Task SaveM3UFile(M3UFile m3uFile, CancellationToken cancellationToken)
    {
        Repository.M3UFile.CreateM3UFile(m3uFile);
        _ = await Repository.SaveAsync().ConfigureAwait(false);
        m3uFile.WriteJSON();

        if (string.IsNullOrEmpty(m3uFile.Url))
        {
            m3uFile.LastDownloaded = DateTime.Now;
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            m3uFile.WriteJSON();
        }


    }
}
