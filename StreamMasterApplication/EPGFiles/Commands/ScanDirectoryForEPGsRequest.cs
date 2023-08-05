using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.EPGFiles.Commands;

public class ScanDirectoryForEPGFilesRequest : IRequest<bool>
{
}

public class ScanDirectoryForEPGFilesRequestHandler : BaseMediatorRequestHandler, IRequestHandler<ScanDirectoryForEPGFilesRequest, bool>
{

    public ScanDirectoryForEPGFilesRequestHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<bool> Handle(ScanDirectoryForEPGFilesRequest command, CancellationToken cancellationToken)
    {
        FileDefinition fd = FileDefinitions.EPG;

        DirectoryInfo EPGDirInfo = new(fd.DirectoryLocation);

        EnumerationOptions er = new()
        {
            MatchCasing = MatchCasing.CaseInsensitive
        };

        IEnumerable<FileInfo> files = EPGDirInfo.GetFiles("*.*", SearchOption.AllDirectories)
           .Where(s => s.FullName.ToLower().EndsWith(fd.FileExtension.ToLower()) || s.FullName.ToLower().EndsWith(fd.FileExtension + ".gz".ToLower()));

        foreach (FileInfo EPGFileInfo in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (EPGFileInfo.DirectoryName == null)
            {
                continue;
            }

            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileBySourceAsync(EPGFileInfo.Name).ConfigureAwait(false);

            if (epgFile == null)
            {
                string Url = "";
                string filePath = Path.Combine(EPGFileInfo.DirectoryName, Path.GetFileNameWithoutExtension(EPGFileInfo.FullName) + ".url");

                if (FileUtil.ReadUrlFromFile(filePath, out string? url))
                {
                    Url = url;
                }

                epgFile = new EPGFile
                {
                    Name = Path.GetFileNameWithoutExtension(EPGFileInfo.Name),
                    Source = EPGFileInfo.Name,
                    Description = $"Imported from {EPGFileInfo.Name}",
                    LastDownloaded = EPGFileInfo.LastWriteTime,
                    LastDownloadAttempt = DateTime.Now,
                    FileExists = true,
                    Url = Url
                };

                epgFile.SetFileDefinition(FileDefinitions.EPG);

                Repository.EPGFile.CreateEPGFile(epgFile);
                await Repository.SaveAsync().ConfigureAwait(false);

            }

            if (string.IsNullOrEmpty(epgFile.Url))
            {
                epgFile.LastDownloaded = EPGFileInfo.LastWriteTime;
                Repository.EPGFile.UpdateEPGFile(epgFile);
                await Repository.SaveAsync().ConfigureAwait(false);
            }

            EPGFilesDto ret = Mapper.Map<EPGFilesDto>(epgFile);
            await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return true;
    }
}
