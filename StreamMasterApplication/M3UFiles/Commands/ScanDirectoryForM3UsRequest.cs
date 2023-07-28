using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.M3UFiles.Commands;

public class ScanDirectoryForM3UFilesRequest : IRequest<bool>
{
}

public class ScanDirectoryForM3UFilesRequestHandler : IRequestHandler<ScanDirectoryForM3UFilesRequest, bool>
{
    private readonly IAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    public ScanDirectoryForM3UFilesRequestHandler(
        IPublisher publisher,
          IMapper mapper,
         IAppDbContext context)
    {
        _publisher = publisher;
        _mapper = mapper;
        _context = context;
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

            M3UFile? m3uFile = await _context.M3UFiles.FirstOrDefaultAsync(a => a.Source.ToLower().Equals(m3uFileInfo.Name.ToLower()), cancellationToken: cancellationToken).ConfigureAwait(false);
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

                _ = _context.M3UFiles.Add(m3uFile);
                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(m3uFile.Url))
            {
                m3uFile.LastDownloaded = m3uFileInfo.LastWriteTime;
                _ = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            M3UFilesDto ret = _mapper.Map<M3UFilesDto>(m3uFile);
            await _publisher.Publish(new M3UFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
        }

        return true;
    }
}
