using StreamMaster.Domain.Color;

namespace StreamMaster.Application.EPGFiles.Commands;

public record ScanDirectoryForEPGFilesRequest : IRequest<bool> { }

public class ScanDirectoryForEPGFilesRequestHandler(ILogger<ScanDirectoryForEPGFilesRequest> Logger, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<ScanDirectoryForEPGFilesRequest, bool>
{
    public async Task<bool> Handle(ScanDirectoryForEPGFilesRequest command, CancellationToken cancellationToken)
    {
        IEnumerable<FileInfo> epgFiles = GetEPGFilesFromDirectory();
        foreach (FileInfo epgFileInfo in epgFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            await ProcessEPGFile(epgFileInfo, cancellationToken);
        }

        return true;
    }

    private IEnumerable<FileInfo> GetEPGFilesFromDirectory()
    {
        FileDefinition fd = FileDefinitions.EPG;
        DirectoryInfo epgDirInfo = new(fd.DirectoryLocation);
        EnumerationOptions er = new() { MatchCasing = MatchCasing.CaseInsensitive };
        string[] extensions = fd.FileExtension.Split('|');

        return epgDirInfo.GetFiles("*.*", SearchOption.AllDirectories)
            .Where(s => extensions.Contains(s.Extension.ToLower()) || extensions.Contains(s.Extension.ToLower() + ".gz"));
    }

    private async Task ProcessEPGFile(FileInfo epgFileInfo, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(epgFileInfo.DirectoryName))
        {
            return;
        }

        EPGFile? epgFile = await Repository.EPGFile.GetEPGFileBySource(epgFileInfo.Name);
        if (epgFile == null)
        {
            epgFile = CreateOrUpdateEPGFile(epgFileInfo);
            await SaveAndPublishEPGFile(epgFile, cancellationToken);
        }

        EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);
        await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
    }

    private EPGFile CreateOrUpdateEPGFile(FileInfo epgFileInfo)
    {
        EPGFile epgFile = EPGFile.ReadJSON(epgFileInfo) ?? new EPGFile
        {
            Name = Path.GetFileNameWithoutExtension(epgFileInfo.Name),
            Source = epgFileInfo.Name,
            Description = $"Imported from {epgFileInfo.Name}",
            FileExists = true,
            LastDownloaded = epgFileInfo.LastWriteTime,
            LastDownloadAttempt = epgFileInfo.LastWriteTime,
            Color = ColorHelper.GetColor(epgFileInfo.Name),
            Url = ""
        };

        if (epgFile.LastUpdated == default)
        {
            epgFile.LastUpdated = DateTime.MinValue;
        }

        epgFile.SetFileDefinition(FileDefinitions.EPG);

        return epgFile;
    }

    private async Task SaveAndPublishEPGFile(EPGFile epgFile, CancellationToken cancellationToken)
    {
        Repository.EPGFile.CreateEPGFile(epgFile);
        _ = await Repository.SaveAsync().ConfigureAwait(false);
        epgFile.WriteJSON(Logger);

        if (string.IsNullOrEmpty(epgFile.Url))
        {
            epgFile.LastDownloaded = DateTime.UtcNow;
            Repository.EPGFile.UpdateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON(Logger);
        }

    }
}